using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace LOBS
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // WindowBase
    // 윈도우 베이스 클래스
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


    public class WindowBase : MonoBehaviour
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Nested Class
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Nested] PopUpBase
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public class NPopUpBase
        {
            public GameObject Root;
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] BaseSetting

        #endregion

        #region [Variable] Window
        protected RectTransform _rectTransform;
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] Base
        //------------------------------------------------------------------------------------------------------------------------------------------------------

        public Vector2 GetSpriteSize(Transform sprite) => sprite.GetComponent<RectTransform>().sizeDelta;
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init] 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        public virtual void Start() { }
        #endregion

        #region [Window] Instantiate WIndow Prefabs
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void InstantiateWindow(GameObject obj, Transform parent)
        {
            Instantiate(obj, parent);
        }
        #endregion

        #region [Window] Open
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void OpenWindow()
        {
            gameObject.SetActive(true);
        }
        public virtual bool IsOpen => (this != null) && (gameObject != null) && gameObject.activeSelf;
        #endregion

        #region [Window] CloseWindow
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void CloseWindow(bool destroy)
        {
            if (destroy)
            {
                Destroy(gameObject);
            }
            else
                gameObject.SetActive(false);
        }
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 1. Tweening
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Tweening] Move
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Move(Transform obj, float duration, Vector3 target)
        {
            obj.DOMove(target, duration);
        }
        #endregion

        #region [Tweening] Move_Dir
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Move_Dir(Transform obj, float duration, UI_Tween_Direction dir, float pos)
        {
            switch (dir)
            {
                case UI_Tween_Direction.X:
                    obj.DOMoveX(pos, duration);
                    break;
                case UI_Tween_Direction.Y:
                    obj.DOMoveY(pos, duration);
                    break;
                case UI_Tween_Direction.Z:
                    obj.DOMoveZ(pos, duration);
                    break;
            }
        }
        #endregion

    }
}
