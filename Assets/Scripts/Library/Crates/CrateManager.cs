using System;
using System.Collections.Generic;
using System.Linq;
using RetroRecord.Engine.Chart;
using UnityEngine;

namespace RetroRecord.Library.Crates
{
    public class CrateManager : MonoBehaviour
    {
        [SerializeField]
        private CrateController[] m_controllers;

        [SerializeField]
        private AlbumLibrary m_albumLibrary;

        [SerializeField]
        private uint m_maxObjectsPerCrate;

        [SerializeField]
        private int m_maxDividersPerCrate;

        [field: SerializeField]
        public Divider m_dividerPrefab { get; private set; }

        [field: SerializeField]
        public AlbumObject m_albumPrefab { get; private set; }

        private void Start()
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

            return;

            void ToNextCrate()
            {
                if (currentObjectsCount == 0)
                    return;
                
                m_controllers[currentCrateIdx].Load(currentAlbums.ToArray(), currentDividers.ToArray(), this);
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

            // foreach (CrateController controller in m_controllers)
            // {
            //     controller.Load(m_albumLibrary.Albums, this);
            // }
        }
    }
}