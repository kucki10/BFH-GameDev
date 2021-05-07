using UnityEngine;

public class Prefs
{
    public float suspensionDistance;

    public int buggyColorHue;
    public int buggyColorSat;
    public int buggyColorVal;

    public float frictionForwards;
    public float frictionSidewards;


    public bool frontLightsEnabled;
    public int frontLightsColorHue;
    public int frontLightsColorSat;
    public int frontLightsColorVal;


    public void Load()
    {
        suspensionDistance = PlayerPrefs.GetFloat(nameof(suspensionDistance), 0.2f);

        buggyColorHue = PlayerPrefs.GetInt(nameof(buggyColorHue), 50);
        buggyColorSat = PlayerPrefs.GetInt(nameof(buggyColorSat), 120);
        buggyColorVal = PlayerPrefs.GetInt(nameof(buggyColorVal), 180);

        frictionForwards = PlayerPrefs.GetFloat(nameof(frictionForwards), 5.0f);
        frictionSidewards = PlayerPrefs.GetFloat(nameof(frictionSidewards), 5.0f);

        frontLightsEnabled = PlayerPrefs.GetInt(nameof(frontLightsEnabled)) == 1;
        frontLightsColorHue = PlayerPrefs.GetInt(nameof(frontLightsColorHue), 0);
        frontLightsColorSat = PlayerPrefs.GetInt(nameof(frontLightsColorSat), 0);
        frontLightsColorVal = PlayerPrefs.GetInt(nameof(frontLightsColorVal), 255);

    }

    public void Save()
    {
        PlayerPrefs.SetFloat(nameof(suspensionDistance), suspensionDistance);

        PlayerPrefs.SetInt(nameof(buggyColorHue), buggyColorHue);
        PlayerPrefs.SetInt(nameof(buggyColorSat), buggyColorSat);
        PlayerPrefs.SetInt(nameof(buggyColorVal), buggyColorVal);

        PlayerPrefs.SetFloat(nameof(frictionForwards), frictionForwards);
        PlayerPrefs.SetFloat(nameof(frictionSidewards), frictionSidewards);

        PlayerPrefs.SetInt(nameof(frontLightsEnabled), frontLightsEnabled ? 1 : 0);
        PlayerPrefs.SetInt(nameof(frontLightsColorHue), frontLightsColorHue);
        PlayerPrefs.SetInt(nameof(frontLightsColorSat), frontLightsColorSat);
        PlayerPrefs.SetInt(nameof(frontLightsColorVal), frontLightsColorVal);

    }


    public void SetAll(ref WheelCollider wheelFL, ref WheelCollider wheelFR, ref WheelCollider wheelRL, ref WheelCollider wheelRR)
    {
        SetWheelColliderSuspension(ref wheelFL, ref wheelFR, ref wheelRL, ref wheelRR);
    }

    public void SetWheelColliderSuspension(ref WheelCollider wheelFL, ref WheelCollider wheelFR, ref WheelCollider wheelRL, ref WheelCollider wheelRR)
    {
        wheelFL.suspensionDistance = suspensionDistance;
        wheelFR.suspensionDistance = suspensionDistance;
        wheelRL.suspensionDistance = suspensionDistance;
        wheelRR.suspensionDistance = suspensionDistance;
    }

    public void SetFriction(ref CarBehaviour carBehaviour)
    {
        carBehaviour.forewardStiffness = frictionForwards;
        carBehaviour.sidewaysStiffness = frictionSidewards;
    }

    public void SetBuggyColor(ref MeshRenderer buggyMeshRenderer)
    {
        var color = GetRgbFromHsv(buggyColorHue, buggyColorSat, buggyColorVal);
        buggyMeshRenderer.material.color = color;
    }

    public void SetFrontLightsEnabled(ref Light frontLight, ref MeshRenderer frontLightsMeshRenderer)
    {
        frontLight.enabled = frontLightsEnabled;
        frontLightsMeshRenderer.enabled = frontLightsEnabled;
    }

    public void SetFrontLightsColor(ref Light frontLight, ref MeshRenderer frontLightsMeshRenderer)
    {
        var color = GetRgbFromHsv(frontLightsColorHue, frontLightsColorSat, frontLightsColorVal);
        frontLight.color = color;
        frontLightsMeshRenderer.material.color = color;
    }


    private Color GetRgbFromHsv(float h, float s, float v)
    {
        float H = h / 180.0f;
        float S = s / 255.0f;
        float V = v / 255.0f;
        var color = Color.HSVToRGB(H, S, V);
        return color;
    }
}
