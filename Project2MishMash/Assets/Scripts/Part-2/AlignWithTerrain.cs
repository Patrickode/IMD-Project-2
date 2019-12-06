using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignWithTerrain : MonoBehaviour
{
    Vector3 terrainNormal;
    public Renderer rend;
    public float maxColorDifference = 1f;

    void Start()
    {
        //When this object is spawned, make sure it's level with the terrain.
        transform.position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position) + 0.5f, transform.position.z);

        //Once level, rotate to be aligned with the terrain.
        terrainNormal = GetTerrainNormal(Terrain.activeTerrain, transform.position);
        transform.forward = transform.forward - terrainNormal * Vector3.Dot(transform.forward, terrainNormal);
        transform.rotation = Quaternion.LookRotation(transform.forward, terrainNormal);

        //As a bonus, randomly change color according to a bell curve.
        GaussianizeColor();
    }

    /// <summary>
    /// Takes a terrain and a position, and gets the normal of the terrain at that position.
    /// Huge thanks to http://answers.unity.com/answers/1295326/view.html for this method; it's been
    /// slightly edited to fit this script, but the idea of using InverseLerp and whatnot came from there.
    /// </summary>
    /// <param name="terrain">The terrain to get the normal from.</param>
    /// <param name="position">The position on the terrain to get the normal from.</param>
    /// <returns>The normal at the given position on the given terrain.</returns>
    private Vector3 GetTerrainNormal(Terrain terrain, Vector3 position)
    {
        //Subtracts the terrain's position from the given position, to get the position relative to the terrain
        Vector3 terrainLocalPos = position - terrain.transform.position;

        //with that relative position, use InverseLerp (get percentage from value) to get how far along the terrain
        //the relative position is, from 0 to 1
        Vector2 normalizedPos = new Vector2(
            Mathf.InverseLerp(0f, terrain.terrainData.size.x, terrainLocalPos.x),
            Mathf.InverseLerp(0f, terrain.terrainData.size.z, terrainLocalPos.z)
        );

        //Finally, now that we have the normalized, local position, we can get the interpolated normal at that position
        var terrainNormal = terrain.terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);
        return terrainNormal;
    }

    /// <summary>
    /// Takes a mean and a standard deviation, and returns a random value according to the bell curve defined by them.
    /// </summary>
    /// <param name="mean">The average value of the bell curve.</param>
    /// <param name="maxDifference">The maximum difference from the mean. Standard deviation is a 4th of this.</param>
    /// <returns>A random value somewhere on the bell curve; likely within 1 deviation.</returns>
    private float Gaussian(float mean, float maxDifference)
    {
        float val1 = Random.Range(0f, 1f);
        float val2 = Random.Range(0f, 1f);

        float gaussValue = Mathf.Sqrt(-2.0f * Mathf.Log(val1)) * Mathf.Sin(2.0f * Mathf.PI * val2);

        return mean + (maxDifference / 8) * gaussValue;
    }

    //Change the color of this object according to a bell curve.
    private void GaussianizeColor()
    {
        //get the HSV values of the current material on this object.
        Color.RGBToHSV(rend.material.color, out float currentHue, out float currentSaturation, out float currentBrightness);

        //Make a color vector with gaussian values.
        Vector4 colorVector = new Vector4
            (
                Gaussian(currentHue, maxColorDifference / 8f),
                Gaussian(currentSaturation, maxColorDifference / 8f),
                Gaussian(currentBrightness, maxColorDifference / 8f),
                rend.material.color.a
            );

        //Make sure those values are valid for a color, i.e., between 0 and 1
        Vector4 gaussianColor = UnderflowValues(colorVector, 0, 1);

        //Finally, set the color to this new gaussian one.
        //Note there is no fourth component; HSVtoRGB only allows opaque colors.
        rend.material.SetColor("_Color", Color.HSVToRGB(gaussianColor.x, gaussianColor.y, gaussianColor.z));
    }

    /// <summary>
    /// Underflows all values below a given bound in the given vector.
    /// </summary>
    /// <param name="baseVector">The vector to underflow the negatives of.</param>
    /// <param name="lowerBound">The desired lower bound of the values.</param>
    /// <param name="upperBound">The desired upper bound of the values.</param>
    /// <returns>The base vector, with all values below the lower bound underflowed.</returns>
    private Vector4 UnderflowValues(Vector4 baseVector, float lowerBound, float upperBound)
    {
        Vector4 underflowedVector = baseVector;

        if (baseVector.x < lowerBound)
        {
            underflowedVector.x = upperBound - baseVector.x;
        }

        if (baseVector.y < lowerBound)
        {
            underflowedVector.y = upperBound - baseVector.y;
        }

        if (baseVector.z < lowerBound)
        {
            underflowedVector.z = upperBound - baseVector.z;
        }

        if (baseVector.w < lowerBound)
        {
            underflowedVector.w = upperBound - baseVector.w;
        }

        return underflowedVector;
    }
}
