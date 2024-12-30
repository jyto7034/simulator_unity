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
        string[] pngFiles = System.IO.Directory.GetFiles(imageFolderPath, "*.png");

        foreach (string pngPath in pngFiles) {
            TextureImporter importer = AssetImporter.GetAtPath(pngPath) as TextureImporter;
            if (importer != null) {
                importer.textureType = TextureImporterType.Sprite;
                importer.SaveAndReimport();
            }
        }
#endif

        int cardIndex = 0;
        foreach (CardJson cardInfo in cardData.cards) {
            // 각 카드에 해당하는 스프라이트 로드
            string spritePath = $"Images/card_img/{cardInfo.name}";
            Sprite cardSprite = Resources.Load<Sprite>(spritePath);

            if (cardSprite == null) {
                Debug.LogError($"Failed to load sprite for card: {cardInfo.name}");
                continue;
            }

            // count만큼 카드 인스턴스 생성
            for (int i = 0; i < cardInfo.count; i++) {
                Material material = new Material(Shader.Find("Sprites/Default"));
                material.mainTexture = cardSprite.texture;

                // 텍스처의 알파 채널 사용 설정
                material.SetFloat("_Mode", 2); // Transparent 모드
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;

                GameObject cardInstance = Instantiate(cardPrefab);
                cardInstance.SetActive(false);
                cardInstance.name = $"{cardInfo.name}_{i}";

                // UV 좌표 반전
                MeshFilter meshFilter = cardInstance.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.mesh != null) {
                    Mesh mesh = meshFilter.mesh;
                    Vector2[] uvs = mesh.uv;
                    for (int j = 0; j < uvs.Length; j++) {
                        uvs[j].y = 1 - uvs[j].y; // Y축 반전
                        uvs[j].x = 1 - uvs[j].x; // X축 반전
                    }

                    mesh.uv = uvs;
                }

                // 머테리얼 적용
                MeshRenderer renderer = cardInstance.GetComponent<MeshRenderer>();
                if (renderer != null) {
                    renderer.material = material;
                }

                // player_type에 따라 리스트에 추가
                if (cardInfo.player_type == 0) {
                    playerTypeZeroCards.Add(cardInstance);
                }
                else {
                    playerTypeOneCards.Add(cardInstance);
                }
            }

            cardIndex++;
        }

        Debug.Log($"Created Type 0 Cards: {playerTypeZeroCards.Count}");
        Debug.Log($"Created Type 1 Cards: {playerTypeOneCards.Count}");

        return (playerTypeZeroCards, playerTypeOneCards);
    }
}