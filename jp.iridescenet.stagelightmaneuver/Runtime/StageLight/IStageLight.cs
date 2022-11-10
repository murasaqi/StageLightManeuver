using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageLightManeuver
{

    public interface IStageLight
    {
        public int Index { get; set; }
        public List<StageLightBase> SyncStageLight { get; set; }
        public void EvaluateQue(float time);
        public void AddStageLightInChild(){}
        public void AddQue(SlmToggleValueBase slmToggleValueBase, float weight){}
        
        public void Init(){}
    }


    public interface IStageLightFixture
    {
        public List<StageLightFixtureBase> StageLightFixtures { get; set; }
        
        
           
        public T TryGetFixture<T>() where T : StageLightFixtureBase
        {
            return StageLightFixtures.FirstOrDefault(x => x is T) as T;
        }
        
    }
 
    
    
    
}
