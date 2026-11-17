using RetroRecord.Engine.Chart;
using UnityEngine;

namespace RetroRecord.Library.Crates
{
    public class CrateController : MonoBehaviour
    {
        private CrateManager m_manager;
        
        private Album[] m_albums;
        private AlbumObject[] m_albumObjects;

        [SerializeField]
        private Vector3 m_albumsStartingPoint;

        [SerializeField]
        private Vector3 m_albumsOffset;

        [SerializeField]
        private Quaternion m_albumsRotation;
        
        [SerializeField]
        private Quaternion m_albumsFlipRotation;
        
        public void Load(Album[] albums, DividerData[] dividerChars, CrateManager manager)
        {
            m_manager = manager;
            m_albums = albums;
            
            if (dividerChars == null || dividerChars.Length == 0)
                SpawnObjects();
            else
                SpawnObjects(dividerChars);
        }

        private void SpawnObjects()
        {
            int albumCount = m_albums.Length;
            m_albumObjects = new AlbumObject[albumCount];
            InstantiateParameters instantiateParams = new() { worldSpace = false, parent = transform };
            for (int i = 0; i < albumCount; i++)
            {
                AlbumObject obj = Instantiate(m_manager.m_albumPrefab, m_albumsStartingPoint + m_albumsOffset * i, m_albumsRotation, instantiateParams);
                m_albumObjects[i] = obj;
                obj.Load(m_albums[i]);
            }
        }

        private void SpawnObjects(DividerData[] dividerChars)
        {
            int objCount = m_albums.Length + dividerChars.Length;
            m_albumObjects = new AlbumObject[m_albums.Length];
            InstantiateParameters instantiateParams = new() { worldSpace = false, parent = transform };
            int dividerIdx = 0;
            for (int i = 0; i < objCount; i++)
            {
                DividerData? currentDividerData = null;
                if (dividerIdx < dividerChars.Length)
                    currentDividerData = dividerChars[dividerIdx];
                if (currentDividerData != null && currentDividerData.Value.Index == i)
                {
                    Divider obj = Instantiate(m_manager.m_dividerPrefab, m_albumsStartingPoint + m_albumsOffset * i,
                        m_albumsRotation, instantiateParams);
                    obj.Load((DividerData)currentDividerData);
                    dividerIdx++;
                }
                else
                {
                    AlbumObject obj = Instantiate(m_manager.m_albumPrefab, m_albumsStartingPoint + m_albumsOffset * i,
                        m_albumsRotation, instantiateParams);
                    m_albumObjects[i - dividerIdx] = obj;
                    obj.Load(m_albums[i - dividerIdx]);
                }
            }
        }
    }
}