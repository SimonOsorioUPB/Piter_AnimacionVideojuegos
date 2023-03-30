using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Sofia_IKSnapper : MonoBehaviour
{
    [SerializeField][Range(0,1)] private float proceduralInfluence;
    [SerializeField] private MultiParentConstraint[] animatedBones;
    [SerializeField] private MultiParentConstraint[] proceduralBones;

    private void UpdateInfluence(float weight)
    {
        if (animatedBones == null) return;
        
        foreach (MultiParentConstraint multiParentConstraint in animatedBones)
        {
            if (multiParentConstraint == null) continue;
            multiParentConstraint.weight = weight;
        }

        if (proceduralBones == null) return;
        
        foreach (MultiParentConstraint proceduralConstraint in proceduralBones)
        {
            if (proceduralConstraint == null) continue;
            proceduralConstraint.weight = 1 - weight;
        }
    }
    
    private void OnValidate()
    {
        UpdateInfluence(proceduralInfluence);
    }
}
