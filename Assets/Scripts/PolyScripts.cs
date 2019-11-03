using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolyToolkit;


public class PolyScripts : MonoBehaviour {
    public InputField searchField;
    public Dropdown categoryDropdown;
    public Dropdown orderDropdown;
    public Slider sizeSlider;
    public Text attributionsText;
    public int assetCount = 0;
    public GameObject assetMenu;    
    private int resultSetSize = 0;

    private List<PolyAsset> fetchedAssets = new List<PolyAsset>();

    public void SearchAPI() {
        if (searchField.text.Length > 0) {
            // Create a new request
            PolyListAssetsRequest request = new PolyListAssetsRequest();

            // Set request parameters
            request.keywords = searchField.text;
            request.pageSize = (int)(sizeSlider.value);

            PolyCategory category = (PolyCategory)categoryDropdown.value;
            request.category = category;

            PolyOrderBy order = (PolyOrderBy)orderDropdown.value;
            request.orderBy = order;

            // Make the request and pass result to callback function
            PolyApi.ListAssets(request, HandleResults);
        }
    }

    private void HandleResults(PolyStatusOr<PolyListAssetsResult> result) {
        // Handle error.
        if (!result.Ok) {
            InterfaceManager.GetInstance().AddMessageToBox("Error fetching asset thumbnail from Poly API :(");
            return;
        }

        assetMenu.GetComponent<PolyMenu>().ClearMenu();
        fetchedAssets.Clear();
        resultSetSize = result.Value.assets.Count;

        foreach (PolyAsset asset in result.Value.assets) {
            fetchedAssets.Add(asset);
            PolyApi.FetchThumbnail(asset, AddToMenu);
        }
    }

    private void AddToMenu(PolyAsset asset, PolyStatus status) {
        assetMenu.GetComponent<PolyMenu>().AddToMenu(asset);        
    }

    private void ImportResults(PolyStatusOr<PolyListAssetsResult> result) {
        // Set options for import so the assets aren't crazy sizes
        PolyImportOptions options = PolyImportOptions.Default();
        options.rescalingMode = PolyImportOptions.RescalingMode.FIT;
        options.desiredSize = 5.0f;
        options.recenter = true;

        // List our assets
        List<PolyAsset> assetsInUse = new List<PolyAsset>();

        // Loop through the list and display the first 3
        for (int i = 0; i < Mathf.Min(5, result.Value.assets.Count); i++) {
            // Import our assets into the scene with the ImportAsset function
            PolyApi.Import(result.Value.assets[i], options, ImportAsset);
            PolyAsset asset = result.Value.assets[i];
            assetsInUse.Add(asset);

            //attributionsText.text = PolyApi.GenerateAttributions(includeStatic: false, runtimeAssets: assetsInUse);
        }

        string attribution = PolyApi.GenerateAttributions(includeStatic: false, runtimeAssets: assetsInUse);

        InterfaceManager.GetInstance().AddMessageToBox(attribution);
    }

    private void ImportAsset(PolyAsset asset, PolyStatusOr<PolyImportResult> result) {
        // Line the assets up so they don't overlap
        GameObject go = result.Value.gameObject;
        go.transform.position = new Vector3(Random.Range(-10.0f, 10.0f), 2f, Random.Range(-10.0f, 10.0f));
        go.AddComponent<MouseDrag>();
        assetCount++;
    }
}