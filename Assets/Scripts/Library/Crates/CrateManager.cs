using System.Collections.Generic;
using System.Linq;
using RetroRecord.Engine.Chart;
using RetroRecord.Library.UI;
using UnityEngine;

namespace RetroRecord.Library.Crates
{
    public class CrateManager : MonoBehaviour
    {
        [SerializeField]
        private CameraManager m_cameraManager;

        [SerializeField]
        private LibraryInputManager m_libraryInputManager;
        
        [SerializeField]
        private CrateController[] m_controllers;

        [SerializeField]
        private AlbumLibrary m_albumLibrary;
        
        [SerializeField]
        private UILpManager m_uiLpManager;

        [SerializeField]
        private uint m_maxObjectsPerCrate;

        [SerializeField]
        private int m_maxDividersPerCrate;

        [field: SerializeField]
        public Divider m_dividerPrefab { get; private set; }

        [field: SerializeField]
        public AlbumObject m_albumPrefab { get; private set; }

        private CrateController m_currentCrate;
        private int m_currentCrateIndex;
        private int m_currentObjectIndex;

        private int m_previousCrateIdx;

        private void Awake()
        {
            if (m_albumLibrary.Albums == null || m_albumLibrary.Albums.Length == 0)
            {
                Debug.LogError("No albums found in AlbumLibrary");
                return;
            }

            string alphabetString = "abcdefhijklmnopqrstuvwxyz";
            string extraCharacters = "";


            Album[] allAlbums = m_albumLibrary.Albums;
            allAlbums = allAlbums.OrderBy(album => album.ArtistName)
                .ThenBy(album => album.AlbumName)
                .ToArray();

            List<Album> mainAlbumsList = new();
            List<Album> extraAlbumsList = new();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Album album in allAlbums)
            {
                if (string.IsNullOrEmpty(album.ArtistName) || string.IsNullOrEmpty(album.AlbumName))
                    continue;

                char firstArtistLetter = char.ToLower(album.ArtistName[0]);

                if (alphabetString.Contains(firstArtistLetter))
                {
                    mainAlbumsList.Add(album);
                    continue;
                }
                extraCharacters += firstArtistLetter;
                extraAlbumsList.Add(album);
            }

            Album[] alphabetAlbums = mainAlbumsList.ToArray();
            Album[] miscAlbums = extraAlbumsList.ToArray();
            
            char currentLetter;
            uint currentCrateIdx = 0;
            int currentObjectsCount = 0;
            int currentDividerCount = 0;
            int currentAlbumIdx = 0;
            List<Album> currentAlbums = new();
            List<DividerData> currentDividers = new();
            foreach (char c in alphabetString)
            {
                if (currentDividerCount >= m_maxDividersPerCrate || currentObjectsCount >= m_maxObjectsPerCrate)
                {
                    ToNextCrate();
                    if (currentCrateIdx == m_controllers.Length)
                        return;
                }
                
                currentLetter = c;
                CreateDivider();

                if (currentAlbumIdx >= alphabetAlbums.Length)
                    continue;
                Album currentAlbum = alphabetAlbums[currentAlbumIdx];
                char firstArtistLetter = char.ToLower(currentAlbum.ArtistName[0]);
                if (currentLetter != firstArtistLetter) continue;
                while (currentAlbumIdx < alphabetAlbums.Length)
                {
                    currentAlbum = alphabetAlbums[currentAlbumIdx];
                    firstArtistLetter = char.ToLower(currentAlbum.ArtistName[0]);
                    if (currentLetter != firstArtistLetter)
                        break;
                    currentAlbums.Add(currentAlbum);
                    currentAlbumIdx++;
                    currentObjectsCount++;
                        
                    if (currentObjectsCount >= m_maxObjectsPerCrate)
                        ToNextCrate();
                }
            }

            if (currentDividerCount >= m_maxDividersPerCrate || currentObjectsCount >= m_maxObjectsPerCrate)
                ToNextCrate();
            
            currentLetter = '-';
            CreateDivider();
            
            currentAlbumIdx = 0;
            foreach (char c in extraCharacters)
            {
                currentLetter = c;

                if (currentAlbumIdx >= miscAlbums.Length)
                    continue;
                Album currentAlbum = miscAlbums[currentAlbumIdx];
                char firstArtistLetter = char.ToLower(currentAlbum.ArtistName[0]);
                if (currentLetter != firstArtistLetter) continue;
                while (currentAlbumIdx < miscAlbums.Length)
                {
                    currentAlbum = miscAlbums[currentAlbumIdx];
                    firstArtistLetter = char.ToLower(currentAlbum.ArtistName[0]);
                    if (currentLetter != firstArtistLetter)
                        break;
                    currentAlbums.Add(currentAlbum);
                    currentAlbumIdx++;
                    currentObjectsCount++;
                        
                    if (currentObjectsCount >= m_maxObjectsPerCrate)
                        ToNextCrate();
                }
            }
            
            ToNextCrate();

            m_uiLpManager.Setup(m_albumLibrary);

            return;

            void ToNextCrate()
            {
                if (currentObjectsCount == 0)
                    return;

                int objIdx = m_currentCrateIndex == currentCrateIdx ? m_currentObjectIndex : -1;
                m_controllers[currentCrateIdx].Load(currentAlbums.ToArray(), currentDividers.ToArray(), this, objIdx);
                currentCrateIdx++;
                currentObjectsCount = 0;
                currentDividerCount = 0;
                currentAlbums.Clear();
                currentDividers.Clear();
            }

            void CreateDivider()
            {
                DividerData data = new()
                {
                    Character = currentLetter,
                    Index = currentObjectsCount
                };
                currentDividers.Add(data);
                currentObjectsCount++;
                currentDividerCount++;
            }
        }

        public CameraPosition GetNextPosition()
        {
            while (true)
            {
                m_currentObjectIndex--;
                if (m_currentObjectIndex >= 0) return GetObjectPositon(true);
                MoveCrate(1);
                
                m_currentObjectIndex = m_currentCrate.LastAlbumIdx;

                if (m_currentObjectIndex < 0) continue;

                return GetObjectPositon(true);
            }
        }

        public CameraPosition GetBackPosition()
        {
            while (true)
            {
                m_currentObjectIndex++;
                if (m_currentCrate.ObjectCount > m_currentObjectIndex) return GetObjectPositon(false);
                MoveCrate(-1);
                
                m_currentObjectIndex = m_currentCrate.FirstAlbumIdx;

                if (m_currentObjectIndex < 0) continue;

                return GetObjectPositon(false);
            }
        }

        private CameraPosition GetObjectPositon(bool wentForward)
        {
            AlbumObject obj = GetCurrentAlbum();
            if (obj == null)
                return wentForward ? GetNextPosition() : GetBackPosition();
            CameraPosition data = obj.GetPositon();

            Vector3 rotation = m_currentCrate.transform.rotation.eulerAngles;
            rotation.y -= 180;
            data.rotation = Quaternion.Euler(rotation);
            
            if (m_previousCrateIdx != m_currentObjectIndex)
                m_controllers[m_previousCrateIdx].FlipAlbums(-1);
            m_previousCrateIdx = m_currentCrateIndex;
            m_controllers[m_previousCrateIdx].FlipAlbums(m_currentObjectIndex);
            
            return data;
        }

        public CameraPosition GetCurrentPosition()
        {
            return GetObjectPositon(true);
        }

        public AlbumObject GetCurrentAlbum()
        {
            return m_controllers[m_currentCrateIndex].GetAlbumObject(m_currentObjectIndex);
        }

        private void MoveCrate(int movement) => SetCrate(m_currentCrateIndex += movement);

        private void SetCrate(int idx)
        {
            m_currentCrateIndex = idx;

            if (m_controllers == null || m_controllers.Length == 0)
                return;
            
            if (m_currentCrateIndex < 0)
                SetCrate(m_controllers.Length - 1);
            
            if (m_currentCrateIndex >= m_controllers.Length)
                SetCrate(0);
            
            m_currentCrate = m_controllers[m_currentCrateIndex];

        }
    }
}