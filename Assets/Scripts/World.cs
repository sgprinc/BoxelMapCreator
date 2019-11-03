using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {
    Tile[,] tiles;
    int width;
    int height;

    public World(int width = 10, int height = 10) {
        this.width = width;
        this.height = height;

        tiles = new Tile[this.width, this.height];

        for (int x = 0; x < width; ++x) {
            for (int y = 0; y < height; ++y) {
                tiles[x,y] = new Tile(this, x, y);
            }
        }

        Debug.Log("World Created (" + this.width + "x" + this.height + ")");
    }

    public Tile GetTile(int x, int y) {
        if (x > width || x < 0 || y > height || y < 0) {
            Debug.LogError("Requested tile (" + x + ":" + y + ") is outside of the terrain.");
            return null;
        }
        return tiles[x, y];
    }
}
