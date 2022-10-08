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
            StageLightFixtures.Add(gameObject.AddComponent<MaterialColorFixture>());
            StageLightFixtures.Add(gameObject.AddComponent<DecalFixture>());
            StageLightFixtures.Add(gameObject.AddComponent<GoboFixture>());
        }

        public override void AddQue(StageLightQueData stageLightQueData, float weight)
        {
            base.AddQue(stageLightQueData, weight);
            foreach (var stageLightFixture in StageLightFixtures)
            {
                stageLightFixture.stageLightDataQueue.Enqueue(stageLightQueData);
            }
        }

        public override void UpdateFixture(float time)
        {
            base.UpdateFixture(time);
            foreach (var stageLightFixture in StageLightFixtures)
            {
                stageLightFixture.UpdateFixture(time);
                stageLightFixture.Index = Index;
            }
        }
        
        [ContextMenu("Find Fixtures")]
        public void FindFixtures()
        {
            StageLightFixtures.Clear();
            StageLightFixtures = GetComponentsInChildren<StageLightFixtureBase>().ToList();
        }
    }
}