using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public class StageLight:StageLightBase, IStageLightFixture
    {
        
        [SerializeReference] private List<StageLightFixtureBase> stageLightFixtures = new List<StageLightFixtureBase>();
        public List<StageLightFixtureBase> StageLightFixtures { get => stageLightFixtures; set => stageLightFixtures = value; }

        [ContextMenu("Init")]
        public override void Init()
        {
            FindFixtures();
            stageLightFixtures.Sort( (a,b) => a.updateOrder.CompareTo(b.updateOrder));
            foreach (var stageLightFixture in StageLightFixtures)
            {
                stageLightFixture.Init();
            }
        }


        private void Start()
        {
            Init();
        }

        public override void AddQue(StageLightQueueData stageLightQueData)
        {
            base.AddQue(stageLightQueData);
            foreach (var stageLightFixture in StageLightFixtures)
            {
                if(stageLightFixture != null)stageLightFixture.stageLightDataQueue.Enqueue(stageLightQueData);
            }
        }

        public override void EvaluateQue(float time)
        {
            base.EvaluateQue(time);
            foreach (var stageLightFixture in StageLightFixtures)
            {
                if (stageLightFixture != null)
                {
                    stageLightFixture.EvaluateQue(time);
                    stageLightFixture.Index = Index;
                }
            }
        }

        public override void UpdateFixture()
        {
            if(stageLightFixtures == null) stageLightFixtures = new List<StageLightFixtureBase>();
            foreach (var stageLightFixture in stageLightFixtures)
            {
                if(stageLightFixture)stageLightFixture.UpdateFixture();
            }
        }


        private void Update()
        {
            // UpdateFixture();
        }

        private void OnDestroy()
        {
            // Debug.Log("On Destroy");
            // for (int i = stageLightFixtures.Count-1; i >=0; i--)
            // {
            //     try
            //     {
            //         if(stageLightFixtures[i]!= null)DestroyImmediate(stageLightFixtures[i]);
            //     }
            //     catch (Exception e)
            //     {
            //         Console.WriteLine(e);
            //         throw;
            //     }
            // }
        }


        [ContextMenu("Find Fixtures")]
        public void FindFixtures()
        {
            if (stageLightFixtures != null)
            {
                StageLightFixtures.Clear();
            }
            else
            {
                stageLightFixtures = new List<StageLightFixtureBase>();
            }
            StageLightFixtures = GetComponents<StageLightFixtureBase>().ToList();
        }
    }
}