using UnityEngine;
using System.Linq;

namespace LOBS
{
    namespace DEBUG
    {
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		//
		// DebugDraw
		//
		// 디버그용 드로우 클래스
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		public class HGizmo
        {
#if UNITY_EDITOR
			static GUIStyle _gUILabel;
#endif
			public static Color color { get => Gizmos.color; set => Gizmos.color = value; }

			public static class DebugDraw
			{
				public static Mesh CapsuleMesh
				{
					get
					{
						if (_capsule == null)
						{
							_capsule = Resources.GetBuiltinResource(typeof(Mesh), "Capsule.fbx") as Mesh;
						}

						return _capsule;
					}
				}
				private static Mesh _capsule;

				public static void DrawCollider(Collider collider, Color color, Gizmo_Sort Sort)
				{
					Color origColor = Gizmos.color;
					Gizmos.color = color;
					Gizmos.matrix = collider.transform.localToWorldMatrix;

					if (collider is BoxCollider)
					{
						BoxCollider boxCollider = (BoxCollider)collider;
						switch (Sort)
						{
							case Gizmo_Sort.Mesh:
								Gizmos.DrawCube(boxCollider.center, boxCollider.size);
								break;
							case Gizmo_Sort.Wire:
								Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
								break;
						}
					}
					else if (collider is SphereCollider)
					{
						SphereCollider sphereCollider = (SphereCollider)collider;
						switch (Sort)
						{
							case Gizmo_Sort.Mesh:
								Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius);
								break;
							case Gizmo_Sort.Wire:
								Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
								break;
						}
					}
					else if (collider is CapsuleCollider)
					{
						CapsuleCollider capsuleCollider = (CapsuleCollider)collider;

						Quaternion rotation;

						switch (capsuleCollider.direction)
						{
							//X axis
							case 0: rotation = Quaternion.AngleAxis(90, Vector3.forward); break;
							//Z axis
							case 2: rotation = Quaternion.AngleAxis(90, Vector3.right); break;
							//Y axis
							default:
							case 1: rotation = Quaternion.identity; break;
						}

						float height = capsuleCollider.height;
						switch (Sort)
						{
							case Gizmo_Sort.Mesh:
								Gizmos.DrawMesh(CapsuleMesh, capsuleCollider.center, rotation, new Vector3(capsuleCollider.radius, height * 0.25f, capsuleCollider.radius));
								break;
							case Gizmo_Sort.Wire:
								Gizmos.DrawWireMesh(CapsuleMesh, capsuleCollider.center, rotation, new Vector3(capsuleCollider.radius, height * 0.25f, capsuleCollider.radius));
								break;
						}
					}
					else if (collider is MeshCollider)
					{
						MeshCollider meshCollider = (MeshCollider)collider;
						switch (Sort)
						{
							case Gizmo_Sort.Mesh:
								Gizmos.DrawMesh(meshCollider.sharedMesh);
								break;
							case Gizmo_Sort.Wire:
								Gizmos.DrawWireMesh(meshCollider.sharedMesh);
								break;
						}
					}

					Gizmos.color = origColor;
					Gizmos.matrix = Matrix4x4.identity;
				}

			}
		}



    }

}

