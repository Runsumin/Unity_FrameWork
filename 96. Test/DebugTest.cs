using UnityEngine;

namespace LOBS
{
    public class DebugTest : MonoBehaviour
    {
        public Collider col;
        private void OnDrawGizmos()
        {
            //DEBUG.DebugDraw.DrawCollider(this.gameObject.GetComponent<Collider>(), Color.red, Gizmo_Sort.Wire);
        }
    }

}


