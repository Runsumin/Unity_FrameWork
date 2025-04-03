using UnityEngine;
using System;

namespace LOBS
{
    namespace RE_Global
    {
        #region [Nested] DataUnit
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [Serializable]
        public class NDataUnit
        {
            public int Index;
            public bool Show;
            public string Name;
            public string NameColor;
            public string Data;
            public string Unit;
            public string Description;
            public string IconPath;
            public DataType Type;
        }
        #endregion

        #region [Nested] Object_Description
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [Serializable]
        public class Object_Description
        {
            public int UniqueID;
            public string Name;
            public string Description;
        }
        #endregion

    }

    namespace RL_Global
    {
        #region [Nested] User_Data
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [Serializable]
        public class User_Data
        {
            public string User_Name;
            public string User_Affiliation; 
        }
        #endregion
    }

    public struct Lowdata
    {

    }
}
