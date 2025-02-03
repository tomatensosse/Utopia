using TMPro;
using UnityEngine;

public class WorldEditorGUI : MonoBehaviour
{
    public static WorldEditorGUI Instance { get; private set; }

    public Canvas canvas;

    public GameObject editorPropertyPanel;
    public GameObject chunkPropertyPanel;

    public TMP_Text chunkPositionText;
    public TMP_Text addingBiomeText;
    public TMP_Text currentChunkBiomeText;
    public TMP_Text currentChunkDensityText;
    public TMP_Text currentChunkMeshText;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        if (WorldEditor.Instance == null)
        {
            return;
        }

        chunkPositionText.text = $"Chunk Position: {WorldEditor.Instance.chunkPosition}";
        addingBiomeText.text = $"Adding Biome: {WorldEditor.Instance.addingBiome?.name ?? "None"}";
        currentChunkBiomeText.text = $"Current Chunk Biome: {WorldEditor.Instance.currentChunk?.biome?.name ?? "None"}";
        currentChunkDensityText.text = $"Current Chunk Density: {WorldEditor.Instance.currentChunk?.isDensityGenerated}";
        currentChunkMeshText.text = $"Current Chunk Mesh: {WorldEditor.Instance.currentChunk?.isMeshGenerated}";
    }

    public void PositionUpdated()
    {
        if (WorldEditor.Instance.currentChunk != null)
        {
            chunkPropertyPanel.SetActive(true);
            editorPropertyPanel.SetActive(false);
        }
        else
        {
            chunkPropertyPanel.SetActive(false);
            editorPropertyPanel.SetActive(true);
        }
    }
}