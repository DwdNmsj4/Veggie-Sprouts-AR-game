using System.Collections.Generic;
using UnityEngine;

namespace WildHarvest
{
    public class PatchController : MonoBehaviour
    {
        [Header("stages")]
        public List<GameObject> greenStages = new List<GameObject>();
        public List<GameObject> redStages   = new List<GameObject>();
        public List<GameObject> orangeStages = new List<GameObject>();
        public List<GameObject> purpleStages = new List<GameObject>();

        [Header("current stage")]
        public List<GameObject> currentStages = new List<GameObject>();

        [Header("harvest stage index")]
        public int harvestStageIndex = 4;

        [Tooltip("bites to harvest")]
        public int bitesToHarvest = 3;

        [Header("visual bunch")]
        public GameObject defaultBunchPrefab;
        public GameObject greenBunchPrefab;
        public GameObject redBunchPrefab;
        public GameObject orangeBunchPrefab;
        public GameObject purpleBunchPrefab;

        [Tooltip("harvest amount")]
        public int harvestAmount = 3;

        [Tooltip("bunch spawn radium")]
        public float bunchSpawnRadius = 0.25f;

        [Tooltip("leaf particles")]
        public GameObject leafParticles;

        [Header("refer")]
        [Tooltip("plant animator")]
        public Animator plantAnimator;

        [Tooltip("farm game manager")]
        public FarmGameManager farmManager;

        [Header("for testing")]
        public VegType seedColor = VegType.Green;
        public int currentBiteCount = 0;
        public bool isMature = false;

        void Awake()
        {
            if (plantAnimator == null)
            {
                plantAnimator = GetComponentInChildren<Animator>();
            }
        }

        void Start()
        {
            //InitRound(seedColor, bitesToHarvest);
        }

        public void InitRound(VegType color, int bitesNeeded)
        {
            seedColor = color;
            bitesToHarvest = Mathf.Max(1, bitesNeeded);
            currentBiteCount = 0;
            isMature = false;

            switch (color)
            {
                case VegType.Green:
                    currentStages = greenStages;
                    break;
                case VegType.Red:
                    currentStages = redStages;
                    break;
                case VegType.Orange:
                    currentStages = orangeStages;
                    break;
                case VegType.Purple:
                    currentStages = purpleStages;
                    break;
                default:
                    currentStages = greenStages;
                    break;
            }

            DisableAllVegetables();
            ActivateStage(0);         

            if (leafParticles != null)
            {
                leafParticles.SetActive(false);
            }

            Debug.Log($"[PatchController] InitRound on {name}, color = {seedColor}, bitesToHarvest = {bitesToHarvest}");
        }

        public void OnBite()
        {
            if (isMature)
                return;

            if (currentStages == null || currentStages.Count == 0)
                return;

            currentBiteCount = Mathf.Clamp(currentBiteCount + 1, 0, bitesToHarvest);

            if (plantAnimator != null)
            {
                plantAnimator.Play("React", 0, 0f);
            }

            int maxStageIndex = Mathf.Clamp(harvestStageIndex, 0, currentStages.Count - 1);
            float t = (float)currentBiteCount / bitesToHarvest;
            int stageIndex = Mathf.Clamp(Mathf.RoundToInt(t * maxStageIndex), 0, maxStageIndex);
            ActivateStage(stageIndex);

            if (currentBiteCount >= bitesToHarvest)
            {
                isMature = true;
                Debug.Log("[PatchController] Become mature, auto harvest now!");
                Harvest();
            }
        }

        public void Harvest()
        {
            Debug.Log("[PatchController] Harvest visuals!");

            if (leafParticles != null)
            {
                leafParticles.SetActive(false);
                leafParticles.SetActive(true);
            }
        
            GameObject prefabToSpawn = GetBunchPrefabBySeedColor();
            if (prefabToSpawn != null)
            {
                int count = Mathf.Max(1, harvestAmount);

                for (int i = 0; i < count; i++)
                {
                    Vector2 offset2D = Random.insideUnitCircle * bunchSpawnRadius;
                    Vector3 spawnPos = transform.position +
                                       new Vector3(offset2D.x, 0.05f, offset2D.y);

                    Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                }
            }

            int maxStage = Mathf.Clamp(harvestStageIndex, 0, currentStages.Count - 1);
            ActivateStage(maxStage);

            if (farmManager != null)
            {
                farmManager.OnPatchHarvested(this);
            }
        }

        private void DisableAllVegetables()
        {
            void DisableList(List<GameObject> list)
            {
                if (list == null) return;
                foreach (var go in list)
                {
                    if (go != null) go.SetActive(false);
                }
            }

            DisableList(greenStages);
            DisableList(redStages);
            DisableList(orangeStages);
            DisableList(purpleStages);
        }

        private void ActivateStage(int index)
        {
            if (currentStages == null || currentStages.Count == 0)
                return;

            for (int i = 0; i < currentStages.Count; i++)
            {
                if (currentStages[i] != null)
                    currentStages[i].SetActive(i == index);
            }
        }

        private GameObject GetBunchPrefabBySeedColor()
        {
            GameObject result = defaultBunchPrefab;

            switch (seedColor)
            {
                case VegType.Green:
                    if (greenBunchPrefab != null) result = greenBunchPrefab;
                    break;
                case VegType.Red:
                    if (redBunchPrefab != null) result = redBunchPrefab;
                    break;
                case VegType.Orange:
                    if (orangeBunchPrefab != null) result = orangeBunchPrefab;
                    break;
                case VegType.Purple:
                    if (purpleBunchPrefab != null) result = purpleBunchPrefab;
                    break;
            }

            return result;
        }


        public void ResetVisual()
        {
            DisableAllVegetables();

            if (leafParticles != null)
            {
                leafParticles.SetActive(false);
            }

            currentBiteCount = 0;
            isMature = false;
        }

#if UNITY_EDITOR
        private void OnMouseDown()
        {
            Harvest();
        }
#endif
    }
}