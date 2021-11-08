using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileableStructureTextureGenerator
{ 
    public static Texture2D GenerateTexture(Texture2D originalTexture, Vector2Int fillColorLocation, Vector2Int bottomLeftInnerSquare, Vector2Int topRightInnerSquare, Vector2Int size, bool rightFull, bool leftFull, bool topFull, bool bottomFull)
    {
        Texture2D outputTexture = new Texture2D(size.x, size.y, TextureFormat.ARGB32, false);
        Graphics.CopyTexture(originalTexture, outputTexture);

        //Top full
        if (topFull)
        {
            for (int i = 0; i < size.x; i++)
            {
                for (int j = topRightInnerSquare.y; j < size.y; j++)
                {
                    outputTexture.SetPixel(i, j, originalTexture.GetPixel(i, j - (size.y - topRightInnerSquare.y)));
                }
            }
        }
        //Bottom full
        if (bottomFull)
        {
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < bottomLeftInnerSquare.y; j++)
                {
                    //Color color = originalTexture.GetPixel(i, j)
                    outputTexture.SetPixel(i, j, originalTexture.GetPixel(i, j + bottomLeftInnerSquare.y));
                }
            }
        }


        //Right
        if (leftFull)
        {
            for (int i = 0; i < bottomLeftInnerSquare.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    outputTexture.SetPixel(i, j, originalTexture.GetPixel(i + bottomLeftInnerSquare.x, j));
                }
            }

        }
        //Left
        if (rightFull)
        {
            for (int i = topRightInnerSquare.x; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    outputTexture.SetPixel(i, j, originalTexture.GetPixel(i - bottomLeftInnerSquare.x, j));
                }
            }

        }



        outputTexture.Apply();
       
        return outputTexture;
    }
}
