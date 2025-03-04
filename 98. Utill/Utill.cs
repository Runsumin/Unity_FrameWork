using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOBS
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // Utill
    // 
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


    public static class Utill
    {
        #region [Utill] Point In Rect
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool MouseCursorinZone(Vector2 pos, RectTransform rect)
        {
            Vector3[] worldCorners = new Vector3[4];
            rect.GetWorldCorners(worldCorners);

            // ���� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ
            Vector2 minScreenPos = RectTransformUtility.WorldToScreenPoint(null, worldCorners[0]); // ���ϴ�
            Vector2 maxScreenPos = RectTransformUtility.WorldToScreenPoint(null, worldCorners[2]); // ����

            // ���콺�� Rect �ȿ� �ִ��� �˻�
            if (pos.x > minScreenPos.x && pos.x < maxScreenPos.x &&
                pos.y > minScreenPos.y && pos.y < maxScreenPos.y)
            {
                return true;
            }
            else
                return false;
        }
        #endregion

        #region [Utill] SetMouseCenterpos
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vector3 SetMousePositionCenter(Vector3 mousepos, Vector2 ScreenSize)
        {
            return new Vector3(mousepos.x - ScreenSize.x / 2, mousepos.y - ScreenSize.y / 2, 0);
        }
        #endregion


        #region [Utill] Change HexToColor 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Color HexToColor(string hex)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(hex, out color))
            {
                return color;
            }
            return Color.white;
        }
        #endregion

        #region [Utill] TextureToSprite
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Sprite TextureToSprite(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        #endregion
    }

}
