using UnityEngine;

namespace Scripts.Gameplay
{
    /// <summary>
    /// Makes the attached object SPEEN (in the y axis)
    /// </summary>
    public class ObjectSPEEN : MonoBehaviour
    {
        [SerializeField] private float speenSpeed = 20f;

        private void Update()
        {
            
            transform.Rotate(new Vector3(0, speenSpeed * Mathf.PI * Time.deltaTime, 0));
        }

        private void OnEnable()
        {
            transform.rotation = Quaternion.identity;
        }

        public void DoISpeen(bool speen)
        {
            gameObject.SetActive(speen);
        }
    }
}