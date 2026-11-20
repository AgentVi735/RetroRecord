using RetroRecord.Engine.Chart;
using UnityEngine;

namespace RetroRecord.Library.Crates
{
    public class CrateController : MonoBehaviour
    {
        private CrateManager m_manager;

        private Album[] m_albums;
        private AlbumObject[] m_albumObjects;

        private CrateObject[] m_objects;

        [SerializeField]
        private Vector3 m_albumsStartingPoint;

        [SerializeField]
        private Vector3 m_albumsOffset;

        public int AlbumCount => m_albumObjects?.Length ?? 0;
        public int ObjectCount => m_objects?.Length ?? 0;
        
        public int FirstAlbumIdx { get; private set; }
        public int LastAlbumIdx { get; private set; }

        public void Load(Album[] albums, DividerData[] dividerChars, CrateManager manager, int selectedIndex)
        {
            m_manager = manager;
            m_albums = albums;

            if (dividerChars == null || dividerChars.Length == 0)
                SpawnObjects(selectedIndex);
            else
                SpawnObjects(dividerChars, selectedIndex);
        }

        private void SpawnObjects(int selectedIndex)
        {
            int albumCount = m_albums.Length;
            m_albumObjects = new AlbumObject[albumCount];
            FirstAlbumIdx = -1;
            LastAlbumIdx = -1;
            InstantiateParameters instantiateParams = new() { worldSpace = false, parent = transform };
            for (int i = 0; i < albumCount; i++)
            {
                AlbumObject obj = Instantiate(m_manager.m_albumPrefab, m_albumsStartingPoint + m_albumsOffset * i,
                    Quaternion.identity, instantiateParams);
                m_albumObjects[i] = obj;
                bool shouldBeFlipped = selectedIndex > 0 && selectedIndex < i;
                obj.Load(m_albums[i], shouldBeFlipped);
                if (FirstAlbumIdx == -1)
                    FirstAlbumIdx = i;
                LastAlbumIdx = i;
            }
        }

        private void SpawnObjects(DividerData[] dividerChars, int selectedIndex)
        {
            int objCount = m_albums.Length + dividerChars.Length;
            m_objects = new CrateObject[objCount];
            m_albumObjects = new AlbumObject[m_albums.Length];
            FirstAlbumIdx = -1;
            LastAlbumIdx = -1;
            InstantiateParameters instantiateParams = new() { worldSpace = false, parent = transform };
            int dividerIdx = 0;
            for (int i = 0; i < objCount; i++)
            {
                DividerData? currentDividerData = null;
                bool shouldBeFlipped = selectedIndex > 0 && selectedIndex < i;
                if (dividerIdx < dividerChars.Length)
                    currentDividerData = dividerChars[dividerIdx];
                if (currentDividerData != null && currentDividerData.Value.Index == i)
                {
                    Divider obj = Instantiate(m_manager.m_dividerPrefab, m_albumsStartingPoint + m_albumsOffset * i,
                        Quaternion.identity, instantiateParams);
                    obj.Load((DividerData)currentDividerData, shouldBeFlipped);
                    m_objects[i] = obj;
                    dividerIdx++;
                }
                else
                {
                    AlbumObject obj = Instantiate(m_manager.m_albumPrefab, m_albumsStartingPoint + m_albumsOffset * i,
                        Quaternion.identity, instantiateParams);
                    m_albumObjects[i - dividerIdx] = obj;
                    m_objects[i] = obj;
                    obj.Load(m_albums[i - dividerIdx], shouldBeFlipped);
                    if (FirstAlbumIdx == -1)
                        FirstAlbumIdx = i;
                    LastAlbumIdx = i;
                }
            }
        }

        public AlbumObject GetAlbumObject(int index)
        {
            if (m_albumObjects == null || m_objects.Length <= index)
                return null;

            CrateObject obj = m_objects[index];
            if (obj.Type != CrateObjectType.Album)
                return null;
            AlbumObject albumObj = (AlbumObject)obj;
            return albumObj.Album == null ? null : albumObj;
        }

        public void FlipAlbums(int currentIndex)
        {
            if (m_objects == null || m_objects.Length == 0)
                return;
            
            if (currentIndex == -1)
            {
                foreach (CrateObject obj in m_objects)
                    obj.Flip(false);
                return;
            }
            

            for (int i = 0; i < m_objects.Length; i++)
                m_objects[i].Flip(i > currentIndex);
        }
    }
}