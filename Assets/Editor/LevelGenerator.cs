using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

/// <summary>
/// Gera as 5 fases do jogo (level 1 a level 5) com layout jogavel,
/// perigos, moedas, portal, camera e luz 2D.
/// Menu: Ferramentas > Gerar Fases do Jogo
/// </summary>
public static class LevelGenerator
{
    private const string ScenesFolder = "Assets/Scenes/";
    private const string CoinPrefabPath = "Assets/Prefabs/Circle.prefab";
    private const string SpikePrefabPath = "Assets/Prefabs/espinho.prefab";
    private const string EnemyPrefabPath = "Assets/Prefabs/Enemy.prefab";
    private const string PlayerPrefabPath = "Assets/Prefabs/Player.prefab";
    private const string EnemyProjectilePrefabPath = "Assets/Prefabs/ProjetilInimigo.prefab";
    private const string WhiteSpritePath = "Assets/Sprites/Utilidades/white.png";

    private static readonly string[] BackgroundPaths =
    {
        "Assets/Sprites/Cenarios/Paisagem Urbana Pixelada de Karakura.png",
        "Assets/Sprites/Cenarios/Entrada da Escola Karakura em Pixel Art.png",
        "Assets/Sprites/Cenarios/Paisagem Lunar de Hueco Mundo.png",
        "Assets/Sprites/Cenarios/Cidade da Soul Society em Pixel Art.png",
        "Assets/Sprites/Cenarios/Palácio Celestial do Rei das Almas.png"
    };

    private static readonly string[] SceneNames =
    {
        "level 1",
        "level2",
        "level 3",
        "level 4",
        "level 5"
    };

    private struct Plat
    {
        public float x, y, w, h;

        public Plat(float x, float y, float w, float h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
    }

    private struct EnemySpot
    {
        public Vector2 position;
        public float left;
        public float right;

        public EnemySpot(float x, float y, float left, float right)
        {
            position = new Vector2(x, y);
            this.left = left;
            this.right = right;
        }
    }

    private class LevelSpec
    {
        public int number;
        public string title;
        public string next;
        public float timeLimit;
        public Vector2 spawn;
        public Plat[] platforms;
        public Vector2[] spikes;
        public Vector2[] coins;
        public EnemySpot[] enemies;
        public Vector2[] shooters;
        public float goalX;
    }

    [MenuItem("Ferramentas/Gerar Fases do Jogo")]
    public static void GenerateAll()
    {
        try
        {
            AssetDatabase.Refresh();

            // A cena level 1 e aberta para extrair o prefab do jogador.
            // Depois ela e regerada junto com as outras.
            GameObject playerPrefab = CreatePlayerPrefab();

            Sprite white = GetWhiteSprite();
            Material litMaterial = GetLitSpriteMaterial();
            GameObject projectilePrefab = CreateEnemyProjectilePrefab(white, litMaterial);

            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

            LevelSpec[] specs = BuildSpecs();

            for (int i = 0; i < specs.Length; i++)
            {
                BuildLevel(specs[i], BackgroundPaths[i], white, litMaterial, playerPrefab, projectilePrefab);
            }

            SetupBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("LEVEL GENERATOR OK - 5 fases geradas.");
        }
        catch (Exception exception)
        {
            Debug.LogError("LEVEL GENERATOR FAILED: " + exception);

            if (Application.isBatchMode)
                EditorApplication.Exit(1);
            else
                throw;
        }
    }

    private static LevelSpec[] BuildSpecs()
    {
        List<LevelSpec> specs = new List<LevelSpec>();

        // ---------------------------------------------------------------- FASE 1
        specs.Add(new LevelSpec
        {
            number = 1,
            title = "Fase 1: Karakura",
            next = "level2",
            timeLimit = 0f,
            spawn = new Vector2(-14f, -0.1f),
            goalX = 70f,
            platforms = new[]
            {
                new Plat(-8f, -2f, 16f, 1f),
                new Plat(7.75f, -2f, 9f, 1f),
                new Plat(20.25f, -2f, 9f, 1f),
                new Plat(32.75f, -2f, 9f, 1f),
                new Plat(45.25f, -2f, 9f, 1f),
                new Plat(57.75f, -2f, 9f, 1f),
                new Plat(68f, -2f, 10f, 1f)
            },
            spikes = new[]
            {
                new Vector2(-4f, -1.25f),
                new Vector2(10f, -1.25f),
                new Vector2(23f, -1.25f),
                new Vector2(36f, -1.25f),
                new Vector2(49f, -1.25f),
                new Vector2(61f, -1.25f)
            },
            coins = new[]
            {
                new Vector2(-14f, -1f),
                new Vector2(-10f, -1f),
                new Vector2(-6f, -1f),
                new Vector2(-2f, -1f),
                new Vector2(5f, -1f),
                new Vector2(18f, -1f),
                new Vector2(31f, -1f),
                new Vector2(44f, -1f),
                new Vector2(56f, -1f),
                new Vector2(66f, -1f)
            },
            enemies = new EnemySpot[0],
            shooters = new Vector2[0]
        });

        // ---------------------------------------------------------------- FASE 2
        specs.Add(new LevelSpec
        {
            number = 2,
            title = "Fase 2: Escola Karakura",
            next = "level 3",
            timeLimit = 0f,
            spawn = new Vector2(-14f, -0.1f),
            goalX = 67f,
            platforms = new[]
            {
                new Plat(-10f, -2f, 12f, 1f),
                new Plat(0f, -2f, 5f, 1f),
                new Plat(6f, -2.5f, 4f, 1f),
                new Plat(13f, -2f, 5f, 1f),
                new Plat(21f, -3f, 5f, 1f),
                new Plat(29f, -2.5f, 4f, 1f),
                new Plat(37f, -2f, 6f, 1f),
                new Plat(46f, -3f, 5f, 1f),
                new Plat(55f, -2f, 5f, 1f),
                new Plat(64f, -2f, 10f, 1f)
            },
            spikes = new[]
            {
                new Vector2(-4f, -1.25f),
                new Vector2(7f, -1.75f),
                new Vector2(31f, -1.75f),
                new Vector2(56f, -1.25f),
                new Vector2(62f, -1.25f)
            },
            coins = new[]
            {
                new Vector2(-13f, -1f),
                new Vector2(-9f, -1f),
                new Vector2(-5f, -1f),
                new Vector2(0f, -1f),
                new Vector2(6f, -1.5f),
                new Vector2(13f, -1f),
                new Vector2(21f, -2f),
                new Vector2(29f, -1.5f),
                new Vector2(37f, -1f),
                new Vector2(64f, -1f)
            },
            enemies = new EnemySpot[0],
            shooters = new Vector2[0]
        });

        // ---------------------------------------------------------------- FASE 3
        specs.Add(new LevelSpec
        {
            number = 3,
            title = "Fase 3: Hueco Mundo",
            next = "level 4",
            timeLimit = 0f,
            spawn = new Vector2(-14f, -0.1f),
            goalX = 66.5f,
            platforms = new[]
            {
                new Plat(-8f, -2f, 16f, 1f),
                new Plat(6f, -2f, 12f, 1f),
                new Plat(20f, -2f, 12f, 1f),
                new Plat(34f, -2f, 14f, 1f),
                new Plat(49f, -2f, 12f, 1f),
                new Plat(63f, -2f, 10f, 1f)
            },
            spikes = new[]
            {
                new Vector2(10f, -1.25f),
                new Vector2(18f, -1.25f),
                new Vector2(30f, -1.25f),
                new Vector2(45f, -1.25f),
                new Vector2(52f, -1.25f),
                new Vector2(57f, -1.25f)
            },
            coins = new[]
            {
                new Vector2(-13f, -1f),
                new Vector2(-8f, -1f),
                new Vector2(-3f, -1f),
                new Vector2(5f, -1f),
                new Vector2(14f, -1f),
                new Vector2(24f, -1f),
                new Vector2(36f, -1f),
                new Vector2(47f, -1f),
                new Vector2(60f, -1f),
                new Vector2(65f, -1f)
            },
            enemies = new[]
            {
                new EnemySpot(4f, -0.5f, 1f, 9f),
                new EnemySpot(34f, -0.5f, 29f, 39f),
                new EnemySpot(60f, -0.5f, 59f, 66f)
            },
            shooters = new Vector2[0]
        });

        // ---------------------------------------------------------------- FASE 4
        specs.Add(new LevelSpec
        {
            number = 4,
            title = "Fase 4: Soul Society",
            next = "level 5",
            timeLimit = 0f,
            spawn = new Vector2(-15f, -0.1f),
            goalX = 67f,
            platforms = new[]
            {
                new Plat(-8f, -2f, 18f, 1f),
                new Plat(8f, -2f, 12f, 1f),
                new Plat(22f, -2f, 12f, 1f),
                new Plat(36f, -2f, 14f, 1f),
                new Plat(51f, -2f, 12f, 1f),
                new Plat(64f, -2f, 10f, 1f)
            },
            spikes = new[]
            {
                new Vector2(5f, -1.25f),
                new Vector2(20f, -1.25f),
                new Vector2(31f, -1.25f),
                new Vector2(48f, -1.25f),
                new Vector2(60f, -1.25f)
            },
            coins = new[]
            {
                new Vector2(-14f, -1f),
                new Vector2(-8f, -1f),
                new Vector2(-2f, -1f),
                new Vector2(8f, -1f),
                new Vector2(15f, -1f),
                new Vector2(24f, -1f),
                new Vector2(37f, -1f),
                new Vector2(50f, -1f),
                new Vector2(62f, -1f),
                new Vector2(67f, -1f)
            },
            enemies = new EnemySpot[0],
            shooters = new[]
            {
                new Vector2(11f, -0.25f),
                new Vector2(40f, -0.25f)
            }
        });

        // ---------------------------------------------------------------- FASE 5
        specs.Add(new LevelSpec
        {
            number = 5,
            title = "Fase 5: Palacio do Rei",
            next = "",
            timeLimit = 60f,
            spawn = new Vector2(-14f, -0.1f),
            goalX = 65f,
            platforms = new[]
            {
                new Plat(-9f, -2f, 14f, 1f),
                new Plat(2f, -2.5f, 5f, 1f),
                new Plat(10f, -2f, 5f, 1f),
                new Plat(18f, -3f, 5f, 1f),
                new Plat(26f, -2.5f, 5f, 1f),
                new Plat(34f, -2f, 6f, 1f),
                new Plat(43f, -3f, 5f, 1f),
                new Plat(52f, -2f, 5f, 1f),
                new Plat(61f, -2f, 12f, 1f)
            },
            spikes = new[]
            {
                new Vector2(-5f, -1.25f),
                new Vector2(11f, -1.25f),
                new Vector2(19f, -2.25f),
                new Vector2(44f, -2.25f),
                new Vector2(53f, -1.25f),
                new Vector2(58f, -1.25f)
            },
            coins = new[]
            {
                new Vector2(-13f, -1f),
                new Vector2(-8f, -1f),
                new Vector2(-3f, -1f),
                new Vector2(2f, -1.5f),
                new Vector2(10f, -1f),
                new Vector2(18f, -2f),
                new Vector2(26f, -1.5f),
                new Vector2(34f, -1f),
                new Vector2(43f, -2f),
                new Vector2(61f, -1f)
            },
            enemies = new[]
            {
                new EnemySpot(35f, -0.5f, 32f, 36f)
            },
            shooters = new[]
            {
                new Vector2(62f, -0.25f)
            }
        });

        return specs.ToArray();
    }

    private static void BuildLevel(LevelSpec spec, string backgroundPath, Sprite white, Material litMaterial,
        GameObject playerPrefab, GameObject projectilePrefab)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateGlobalLight();

        // ------------------------------------------------------------ Jogador
        GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
        player.name = "player";
        player.transform.position = new Vector3(spec.spawn.x, spec.spawn.y, 0f);

        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
        if (playerSprite != null)
            playerSprite.sortingOrder = 2;

        // ------------------------------------------------------------ Camera
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";

        Camera cameraComponent = cameraObject.AddComponent<Camera>();
        cameraComponent.orthographic = true;
        cameraComponent.orthographicSize = 6f;
        cameraComponent.clearFlags = CameraClearFlags.SolidColor;
        cameraComponent.backgroundColor = new Color(0.05f, 0.06f, 0.12f, 1f);

        cameraObject.transform.position = new Vector3(spec.spawn.x, spec.spawn.y + 1f, -10f);
        cameraObject.AddComponent<AudioListener>();
        cameraObject.AddComponent<UniversalAdditionalCameraData>();

        cameracontroller follow = cameraObject.AddComponent<cameracontroller>();
        follow.playerTransform = player.transform;
        follow.offset = new Vector3(0f, 1f, 0f);
        follow.smoothTime = 0.15f;

        // ------------------------------------------------------------ Fundo
        Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(backgroundPath);

        if (backgroundSprite != null)
        {
            GameObject background = new GameObject("Fundo");
            background.transform.SetParent(cameraObject.transform, false);
            background.transform.localPosition = new Vector3(0f, 0f, 10f);

            SpriteRenderer backgroundRenderer = background.AddComponent<SpriteRenderer>();
            backgroundRenderer.sprite = backgroundSprite;
            backgroundRenderer.sharedMaterial = litMaterial;
            backgroundRenderer.sortingOrder = -100;

            Vector2 spriteSize = backgroundSprite.bounds.size;
            float viewHeight = 12f;
            float viewWidth = viewHeight * (16f / 9f) + 2f;
            float scale = Mathf.Max(viewWidth / spriteSize.x, viewHeight / spriteSize.y) * 1.1f;

            background.transform.localScale = new Vector3(scale, scale, 1f);
        }
        else
        {
            Debug.LogWarning("Fundo nao encontrado: " + backgroundPath);
        }

        // ------------------------------------------------------------ Plataformas
        Color bodyColor;
        Color topColor;

        switch (spec.number)
        {
            case 2:
                bodyColor = new Color(0.28f, 0.32f, 0.42f);
                topColor = new Color(0.55f, 0.62f, 0.75f);
                break;
            case 3:
                bodyColor = new Color(0.33f, 0.2f, 0.45f);
                topColor = new Color(0.7f, 0.45f, 0.9f);
                break;
            case 4:
                bodyColor = new Color(0.45f, 0.4f, 0.32f);
                topColor = new Color(0.85f, 0.75f, 0.5f);
                break;
            case 5:
                bodyColor = new Color(0.5f, 0.42f, 0.2f);
                topColor = new Color(1f, 0.85f, 0.4f);
                break;
            default:
                bodyColor = new Color(0.32f, 0.25f, 0.2f);
                topColor = new Color(0.42f, 0.62f, 0.35f);
                break;
        }

        foreach (Plat plat in spec.platforms)
            CreatePlatform(plat, white, litMaterial, bodyColor, topColor);

        // ------------------------------------------------------------ Espinhos
        GameObject spikePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SpikePrefabPath);

        foreach (Vector2 position in spec.spikes)
        {
            GameObject spike = (GameObject)PrefabUtility.InstantiatePrefab(spikePrefab);
            spike.name = "Espinho";
            spike.transform.position = new Vector3(position.x, position.y, 0f);
            spike.transform.localScale = Vector3.one * 1.5f;

            SpriteRenderer renderer = spike.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = 3;
                renderer.sharedMaterial = litMaterial;
            }
        }

        // ------------------------------------------------------------ Moedas
        GameObject coinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CoinPrefabPath);

        foreach (Vector2 position in spec.coins)
        {
            GameObject coin = (GameObject)PrefabUtility.InstantiatePrefab(coinPrefab);
            coin.name = "Moeda";
            coin.transform.position = new Vector3(position.x, position.y, 0f);
            coin.transform.localScale = new Vector3(0.7f, -0.7f, 0.7f);

            SpriteRenderer renderer = coin.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = 5;
                renderer.sharedMaterial = litMaterial;
            }
        }

        // ------------------------------------------------------------ Inimigos
        GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EnemyPrefabPath);

        foreach (EnemySpot spot in spec.enemies)
        {
            GameObject enemy = (GameObject)PrefabUtility.InstantiatePrefab(enemyPrefab);
            enemy.name = "Inimigo";
            enemy.transform.position = new Vector3(spot.position.x, spot.position.y, 0f);
            enemy.transform.localScale = Vector3.one * 2f;

            SpriteRenderer renderer = enemy.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = 1;
                renderer.sharedMaterial = litMaterial;
            }

            GameObject leftPoint = new GameObject("Ponto Esquerdo");
            leftPoint.transform.position = new Vector3(spot.left, spot.position.y, 0f);

            GameObject rightPoint = new GameObject("Ponto Direito");
            rightPoint.transform.position = new Vector3(spot.right, spot.position.y, 0f);

            EnemyController controller = enemy.GetComponent<EnemyController>();

            if (controller != null)
            {
                controller.position1 = leftPoint.transform;
                controller.position2 = rightPoint.transform;
                controller.velocity = 2.2f;
                controller.minimumDistance = 0.4f;

                PrefabUtility.RecordPrefabInstancePropertyModifications(controller);
                EditorUtility.SetDirty(controller);
            }
        }

        // ------------------------------------------------------------ Atiradores
        foreach (Vector2 position in spec.shooters)
            CreateShooter(position, white, litMaterial, enemyPrefab, projectilePrefab);

        // ------------------------------------------------------------ Abismo
        GameObject fallZone = new GameObject("Abismo");
        fallZone.transform.position = new Vector3(0f, -14f, 0f);

        BoxCollider2D fallCollider = fallZone.AddComponent<BoxCollider2D>();
        fallCollider.isTrigger = true;
        fallCollider.size = new Vector2(500f, 4f);

        fallZone.AddComponent<FallZone>();

        // ------------------------------------------------------------ Portal
        GameObject goal = new GameObject("Portal");
        goal.transform.position = new Vector3(spec.goalX, 0.3f, 0f);
        goal.transform.localScale = new Vector3(2f, 4f, 1f);

        SpriteRenderer goalRenderer = goal.AddComponent<SpriteRenderer>();
        goalRenderer.sprite = white;
        goalRenderer.color = new Color(0.2f, 1f, 0.45f, 0.85f);
        goalRenderer.sortingOrder = 4;
        goalRenderer.sharedMaterial = litMaterial;

        BoxCollider2D goalCollider = goal.AddComponent<BoxCollider2D>();
        goalCollider.isTrigger = true;

        goal.AddComponent<LevelGoal>();

        // ------------------------------------------------------------ Configuracao
        GameObject configObject = new GameObject("LevelConfig");
        LevelConfig config = configObject.AddComponent<LevelConfig>();
        config.levelNumber = spec.number;
        config.levelTitle = spec.title;
        config.totalCoins = spec.coins.Length;
        config.timeLimit = spec.timeLimit;
        config.nextScene = spec.next;

        // ------------------------------------------------------------ Salvar
        string scenePath = ScenesFolder + SceneNames[spec.number - 1] + ".unity";

        EditorSceneManager.MarkSceneDirty(scene);
        bool saved = EditorSceneManager.SaveScene(scene, scenePath);

        if (!saved)
            throw new Exception("Nao foi possivel salvar a cena " + scenePath);

        Debug.Log("Fase " + spec.number + " gerada: " + scenePath);
    }

    private static void CreatePlatform(Plat plat, Sprite white, Material litMaterial, Color bodyColor, Color topColor)
    {
        GameObject platform = new GameObject("Plataforma");
        platform.layer = 6;
        platform.transform.position = new Vector3(plat.x, plat.y, 0f);

        BoxCollider2D collider = platform.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(plat.w, plat.h);

        GameObject body = new GameObject("Corpo");
        body.transform.SetParent(platform.transform, false);
        body.transform.localScale = new Vector3(plat.w, plat.h, 1f);

        SpriteRenderer bodyRenderer = body.AddComponent<SpriteRenderer>();
        bodyRenderer.sprite = white;
        bodyRenderer.color = bodyColor;
        bodyRenderer.sortingOrder = 0;
        bodyRenderer.sharedMaterial = litMaterial;

        GameObject top = new GameObject("Topo");
        top.transform.SetParent(platform.transform, false);
        top.transform.localPosition = new Vector3(0f, plat.h / 2f - 0.09f, 0f);
        top.transform.localScale = new Vector3(plat.w, 0.18f, 1f);

        SpriteRenderer topRenderer = top.AddComponent<SpriteRenderer>();
        topRenderer.sprite = white;
        topRenderer.color = topColor;
        topRenderer.sortingOrder = 1;
        topRenderer.sharedMaterial = litMaterial;
    }

    private static void CreateShooter(Vector2 position, Sprite white, Material litMaterial, GameObject enemyPrefab,
        GameObject projectilePrefab)
    {
        GameObject shooter = new GameObject("Atirador");
        shooter.tag = "Enemy";
        shooter.transform.position = new Vector3(position.x, position.y, 0f);
        shooter.transform.localScale = Vector3.one * 2.5f;

        SpriteRenderer renderer = shooter.AddComponent<SpriteRenderer>();
        SpriteRenderer enemyRenderer = enemyPrefab.GetComponent<SpriteRenderer>();
        renderer.sprite = enemyRenderer != null ? enemyRenderer.sprite : white;
        renderer.color = new Color(0.9f, 0.35f, 0.35f);
        renderer.sortingOrder = 1;
        renderer.sharedMaterial = litMaterial;

        BoxCollider2D collider = shooter.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        // Contato com o atirador tambem mata (motivo "inimigo").
        shooter.AddComponent<Spike>();

        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(shooter.transform, false);
        firePoint.transform.localPosition = new Vector3(0.55f, 0.15f, 0f);

        EnemyShooter enemyShooter = shooter.AddComponent<EnemyShooter>();

        SerializedObject serialized = new SerializedObject(enemyShooter);
        serialized.FindProperty("projectilePrefab").objectReferenceValue = projectilePrefab;
        serialized.FindProperty("firePoint").objectReferenceValue = firePoint.transform;
        serialized.FindProperty("range").floatValue = 16f;
        serialized.FindProperty("cooldown").floatValue = 1.6f;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateGlobalLight()
    {
        GameObject lightObject = new GameObject("Global Light 2D");
        Light2D light = lightObject.AddComponent<Light2D>();
        light.lightType = Light2D.LightType.Global;
        light.color = Color.white;
        light.intensity = 1f;
    }

    private static Sprite GetWhiteSprite()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Sprites"))
            AssetDatabase.CreateFolder("Assets", "Sprites");

        if (!AssetDatabase.IsValidFolder("Assets/Sprites/Utilidades"))
            AssetDatabase.CreateFolder("Assets/Sprites", "Utilidades");

        if (!File.Exists(WhiteSpritePath))
        {
            Texture2D texture = new Texture2D(8, 8, TextureFormat.RGBA32, false);
            Color32[] pixels = new Color32[64];

            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = new Color32(255, 255, 255, 255);

            texture.SetPixels32(pixels);
            texture.Apply();

            File.WriteAllBytes(WhiteSpritePath, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);

            AssetDatabase.ImportAsset(WhiteSpritePath, ImportAssetOptions.ForceSynchronousImport);

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(WhiteSpritePath);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 8f;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(WhiteSpritePath);
    }

    private static Material GetLitSpriteMaterial()
    {
        GameObject coinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CoinPrefabPath);

        if (coinPrefab != null)
        {
            SpriteRenderer renderer = coinPrefab.GetComponent<SpriteRenderer>();

            if (renderer != null && renderer.sharedMaterial != null)
                return renderer.sharedMaterial;
        }

        return null;
    }

    private static GameObject CreateEnemyProjectilePrefab(Sprite white, Material litMaterial)
    {
        AssetDatabase.DeleteAsset(EnemyProjectilePrefabPath);

        GameObject projectile = new GameObject("ProjetilInimigo");
        projectile.transform.localScale = Vector3.one * 0.8f;

        SpriteRenderer renderer = projectile.AddComponent<SpriteRenderer>();
        renderer.sprite = white;
        renderer.color = new Color(1f, 0.25f, 0.2f);
        renderer.sortingOrder = 4;
        renderer.sharedMaterial = litMaterial;

        CircleCollider2D collider = projectile.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.4f;

        Rigidbody2D body = projectile.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        projectile.AddComponent<EnemyProjectile>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(projectile, EnemyProjectilePrefabPath);
        UnityEngine.Object.DestroyImmediate(projectile);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return prefab;
    }

    private static GameObject CreatePlayerPrefab()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenesFolder + "level 1.unity");
        GameObject player = GameObject.Find("player");

        if (player == null)
            throw new Exception("Objeto 'player' nao encontrado em level 1.unity");

        PlatformController controller = player.GetComponent<PlatformController>();

        if (controller != null)
        {
            SerializedObject serialized = new SerializedObject(controller);
            SerializedProperty coinText = serialized.FindProperty("coinText");

            if (coinText != null)
                coinText.objectReferenceValue = null;

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        if (player.GetComponent<PlayerHealth>() == null)
            player.AddComponent<PlayerHealth>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(player, PlayerPrefabPath);

        // Salva a cena modificada (ela sera regerada logo em seguida).
        EditorSceneManager.SaveScene(scene);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return prefab;
    }

    private static void SetupBuildSettings()
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
        scenes.Add(new EditorBuildSettingsScene(ScenesFolder + "Menu.unity", true));
        scenes.Add(new EditorBuildSettingsScene(ScenesFolder + "level 1.unity", true));
        scenes.Add(new EditorBuildSettingsScene(ScenesFolder + "level2.unity", true));
        scenes.Add(new EditorBuildSettingsScene(ScenesFolder + "level 3.unity", true));
        scenes.Add(new EditorBuildSettingsScene(ScenesFolder + "level 4.unity", true));
        scenes.Add(new EditorBuildSettingsScene(ScenesFolder + "level 5.unity", true));

        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
