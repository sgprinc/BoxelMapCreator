using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolyToolkit;

public class PolyMenu : MonoBehaviour {
    private List<PolyAsset> assetsInMenu = new List<PolyAsset>();

    [SerializeField]
    private GameObject menuItemTemplate;
    [SerializeField]
    private GridLayoutGroup gridGroup;

    public GameObject content;
    private GameObject selectedItem;
    private PolyAsset selectedAsset;

    SceneInitializer scene;
    InterfaceManager ui;

    private void Start() {
        scene = SceneInitializer.GetInstance();
        ui = InterfaceManager.GetInstance();
    }

    public void AddToMenu(PolyAsset asset) {
        if (asset.thumbnailTexture != null) {
            //Add asset to menu list.
            assetsInMenu.Add(asset);

            //Instantiate a "card" to represent the asset in the menu
            GameObject newItem = Instantiate(menuItemTemplate) as GameObject;
            newItem.name = "MenuItem - " + asset.displayName;
            newItem.SetActive(true);
            
            //Get the asset thumbnail
            Texture2D tex = asset.thumbnailTexture;
            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

            //Set the cards elements
            Transform assetTransform = newItem.transform;
            Image itemImage = assetTransform.Find("Thumbnail").gameObject.GetComponent<Image>();
            itemImage.sprite = sprite;
            Text itemName = assetTransform.Find("Name").gameObject.GetComponent<Text>();
            itemName.text = asset.displayName;
            Text itemAuthor = assetTransform.Find("Author").gameObject.GetComponent<Text>();
            itemAuthor.text = asset.authorName;

            //Set the OnClick function
            newItem.GetComponent<Button>().onClick.AddListener(() => MenuItemClicked(asset));

            //Set "content" as parent.
            assetTransform.SetParent(content.transform, false);
        }
    }

    private void MenuItemClicked(PolyAsset asset) {
        if (selectedAsset == asset) {
            return;
        }

        ui.AddMessageToBox(asset.displayName + " selected");
        selectedAsset = asset;                

        // Set options for import so the assets aren't crazy sizes
        PolyImportOptions options = PolyImportOptions.Default();
        options.rescalingMode = PolyImportOptions.RescalingMode.FIT;
        options.desiredSize = 1.0f;
        options.recenter = true;

        PolyApi.Import(asset, options, ParseToGameObject);
    }

    private void ParseToGameObject(PolyAsset asset, PolyStatusOr<PolyImportResult> result) {
        // Line the assets up so they don't overlap
        ui.AddMessageToBox("- Selection assets fetched");
        GameObject newSelectedObject = result.Value.gameObject;
        newSelectedObject.name = asset.displayName;
        scene.SetObjectToBePlaced(newSelectedObject);
        scene.SetTool("place");
    }

    public void ClearMenu() {
        // Clear the local asset list.
        assetsInMenu.Clear();

        // Destroy all GameObjects within content.
        foreach (Transform child in content.transform) {
            Destroy(child.gameObject);
        }
    }
}
