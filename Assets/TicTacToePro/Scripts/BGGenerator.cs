using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TicTacToePro.Pooling;

namespace TicTacToePro
{
    [ExecuteAlways]
    [RequireComponent(typeof(ObjectPooler))]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class BGGenerator : UIBehaviour
    {
        public bool StartSetupGame;
        public bool StartSetupEditor;
        public bool ResizeOnResize;
        public bool GenerateOnResize;
        public int ImagesPerWidth = 5;

        public PooledObject BaseImage;
        public Sprite[] Sprites;
        GridLayoutGroup _grid;
        RectTransform RectTransform;
        ObjectPooler _pooler;
        bool _scheduleNextUpdate;

        protected override void Start()
        {
            if (Application.isPlaying && StartSetupGame || !Application.isPlaying && StartSetupEditor)
                Generate();

            //LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
        }

        protected override void Awake()
        {
            _grid = GetComponent<GridLayoutGroup>();
            _pooler = GetComponent<ObjectPooler>();
            RectTransform = (RectTransform)transform;
        }

        protected override void OnRectTransformDimensionsChange() => Resized();

        void Update()
        {
            if (_scheduleNextUpdate)
            {
                _scheduleNextUpdate = false;
                Generate();
            }
        }

        public void Resized()
        {
            if (GenerateOnResize && !CanvasUpdateRegistry.IsRebuildingLayout())
                Generate();

            _scheduleNextUpdate = CanvasUpdateRegistry.IsRebuildingLayout();

            if (ResizeOnResize)
                Resize();
        }

        void Resize()
        {
            if (RectTransform == null)
                RectTransform = (RectTransform)transform;
            if (_grid == null)
                _grid = GetComponent<GridLayoutGroup>();
            Vector2 res = RectTransform.rect.size;

            var gridSize = res.x / ImagesPerWidth;
            _grid.cellSize = new Vector2(gridSize - _grid.spacing.x, gridSize - _grid.spacing.y);
        }


        [Button(nameof(Generate))] public bool generate;

        void Generate()
        {
            if (_grid == null)
                _grid = GetComponent<GridLayoutGroup>();
            if (RectTransform == null)
                RectTransform = (RectTransform)transform;
            if (_pooler == null)
                _pooler = GetComponent<ObjectPooler>();

            Vector2 res = RectTransform.rect.size;

            var gridSize = res.x / ImagesPerWidth;
            _grid.cellSize = new Vector2(gridSize - _grid.spacing.x, gridSize - _grid.spacing.y);

            var widthCount = Mathf.CeilToInt(RectTransform.rect.width / gridSize);
            var heightCount = Mathf.CeilToInt(RectTransform.rect.height / gridSize);
            var count = widthCount * heightCount;

            _pooler.DisableAllPooledObjects();

            for (int i = 0; i < count; i++)
            {
                var image = _pooler.GetPooledObject(BaseImage);
                var randomIndex = Random.Range(0, Sprites.Length);
                var randomSprite = Sprites[randomIndex];
                image.GetComponent<Image>().sprite = randomSprite;
                image.SetActive(true);
            }
        }

        private static Vector2 GetScreenSize()
        {
            var res = new Vector2();
            if (Application.isPlaying)
                res = new Vector2(Screen.width, Screen.height);
            else
                res = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
            return res;
        }
    }
}