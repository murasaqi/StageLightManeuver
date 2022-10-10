using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public class MovingStageLight:StageLight, IStageLightFixture
    {
        
        [SerializeReference] private List<StageLightFixtureBase> stageLightFixtures;
        public List<StageLightFixtureBase> StageLightFixtures { get => stageLightFixtures; set => stageLightFixtures = value; }

        [ContextMenu("Init")]
        public void Init()
        {
            FindFixtures();
            stageLightFixtures.Sort( (a,b) => a.updateOrder.CompareTo(b.updateOrder));
        }


        private void Start()
        {
            Init();
        }

        [ContextMenu("Apply Fixture Set")]
        public void ApplyBaseFixtureSet()
        {
            for (int i = StageLightFixtures.Count-1; i >= 0; i--)  
            {
                DestroyImmediate(StageLightFixtures[i]);
            }
            StageLightFixtures.Clear();

            var pan = gameObject.AddComponent<LightPanFixture>();
            StageLightFixtures.Add(pan);
            var tilt = gameObject.AddComponent<LightTiltFixture>();
            StageLightFixtures.Add(tilt);
            StageLightFixtures.Add(gameObject.AddComponent<LightFixture>());
            StageLightFixtures.Add(gameObject.AddComponent<SyncLightMaterialFixture>());
            StageLightFixtures.Add(gameObject.AddComponent<DecalFixture>());
            StageLightFixtures.Add(gameObject.AddComponent<GoboFixture>());
        }
        public override void AddQue(StageLightQueData stageLightQueData)
        {
            base.AddQue(stageLightQueData);
            foreach (var stageLightFixture in StageLightFixtures)
            {
                stageLightFixture.stageLightDataQueue.Enqueue(stageLightQueData);
            }
        }

        public override void EvaluateQue(float time)
        {
            base.EvaluateQue(time);
            foreach (var stageLightFixture in StageLightFixtures)
            {
                stageLightFixture.EvaluateQue(time);
                stageLightFixture.Index = Index;
            }
        }

        public void UpdateFixture()
        {
            foreach (var stageLightFixture in stageLightFixtures)
            {
                stageLightFixture.UpdateFixture();
            }
        }


        private void Update()
        {
            UpdateFixture();
        }


        [ContextMenu("Find Fixtures")]
        public void FindFixtures()
        {
            StageLightFixtures.Clear();
            StageLightFixtures = GetComponentsInChildren<StageLightFixtureBase>().ToList();
        }
    }
}