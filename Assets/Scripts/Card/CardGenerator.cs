using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

[System.Serializable]
public class CardJson {
    public int player_type;
    public int count;
    public string name;
}

[System.Serializable]
public class CardData {
    public List<CardJson> cards;
}

public class CardGenerator : MonoBehaviour {
    private static readonly int Mode = Shader.PropertyToID("_Mode");
    private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
    private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
    private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");

    public static (List<GameObject>, List<GameObject>) CreateMaterialsAndApplyToPrefab(GameObject cardPrefab) {
        List<GameObject> playerTypeZeroCards = new List<GameObject>();
        List<GameObject> playerTypeOneCards = new List<GameObject>();

        // JSON 파일 로드
        TextAsset jsonFile = Resources.Load<TextAsset>("card_json");
        if (jsonFile == null) {
            Debug.LogError("Failed to load card_json file");
            return (playerTypeZeroCards, playerTypeOneCards);
        }

        CardData cardData = JsonUtility.FromJson<CardData>(jsonFile.text);

        // PNG 이미지들을 Sprite로 변환
#if UNITY_EDITOR
        string imageFolderPath = "Assets/Resources/Images/card_img";
        string[] pngFiles = Directory.GetFiles(imageFolderPath, "*.png");

        foreach (string pngPath in pngFiles) {
            TextureImporter importer = AssetImporter.GetAtPath(pngPath) as TextureImporter;
            if (importer != null) {
                importer.textureType = TextureImporterType.Sprite;
                importer.SaveAndReimport();
            }
        }
#endif

        string backCardPath = "Images/card_img/card_back";
        Sprite backCardSprite = Resources.Load<Sprite>(backCardPath);
        Material backMaterial = null;

        if (backCardSprite != null) {
            backMaterial = new Material(Shader.Find("Sprites/Default"));
            backMaterial.mainTexture = backCardSprite.texture;
            SetupTransparentMaterial(backMaterial);
        } else {
            Debug.LogError("Failed to load back card sprite");
            return (playerTypeZeroCards, playerTypeOneCards);
        }
        
        foreach (CardJson cardInfo in cardData.cards) {
            string spritePath = $"Images/card_img/{cardInfo.name}";
            Sprite cardSprite = Resources.Load<Sprite>(spritePath);

            if (cardSprite == null) {
                Debug.LogError($"Failed to load sprite for card: {cardInfo.name}");
                continue;
            }

            for (int i = 0; i < cardInfo.count; i++) {
                Material frontMaterial = new Material(Shader.Find("Sprites/Default"));
                frontMaterial.mainTexture = cardSprite.texture;
                SetupTransparentMaterial(frontMaterial);

                GameObject cardInstance = Instantiate(cardPrefab);
                Card.Card cardComponent = cardInstance.GetComponent<Card.Card>();
                
                // UV 좌표 반전 코드는 그대로 유지
                SetupUVCoordinates(cardInstance);

                // 머테리얼 설정
                MeshRenderer renderer = cardInstance.GetComponent<MeshRenderer>();
                if (renderer != null) {
                    renderer.material = frontMaterial;
                }
                
                // Card 컴포넌트에 머테리얼 설정
                cardComponent.SetupMaterials(frontMaterial, backMaterial);
                cardComponent.SetCardStatus(CardStatus.Close);
                
                cardInstance.SetActive(false);
                cardInstance.name = $"{cardInfo.name}_{i}";

                if (cardInfo.player_type == 0) {
                    playerTypeZeroCards.Add(cardInstance);
                } else {
                    playerTypeOneCards.Add(cardInstance);
                }
            }
        }

        Debug.Log($"Created Type 0 Cards: {playerTypeZeroCards.Count}");
        Debug.Log($"Created Type 1 Cards: {playerTypeOneCards.Count}");

        return (playerTypeZeroCards, playerTypeOneCards);
    }
    
    private static void SetupTransparentMaterial(Material material) {
        material.SetFloat(Mode, 2);
        material.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt(ZWrite, 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }
    
    private static void SetupUVCoordinates(GameObject cardInstance) {
        MeshFilter meshFilter = cardInstance.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.mesh != null) {
            Mesh mesh = meshFilter.mesh;
            Vector2[] uvs = mesh.uv;
            for (int j = 0; j < uvs.Length; j++) {
                uvs[j].y = 1 - uvs[j].y;
                uvs[j].x = 1 - uvs[j].x;
            }
            mesh.uv = uvs;
        }
    }
}