    using UnityEngine;

    public class AnimationTrigger : MonoBehaviour
    {
        private Animator animator;

        void Start()
        {
            // Get the Animator component attached to this GameObject
            animator = GetComponent<Animator>(); 
            // Or, if the Animator is on a child object, use:
            // animator = GetComponentInChildren<Animator>();
        }

        void OnTriggerEnter(Collider other)
        {
            // Check if the entering collider belongs to the desired object (e.g., by tag)
            if (other.CompareTag("MainCamera")) 
            {
                // Trigger the animation parameter in the Animator
                animator.SetTrigger("PlayAnimation"); 
            }
        }
    }