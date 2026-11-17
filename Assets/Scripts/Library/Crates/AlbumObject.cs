using RetroRecord.Engine.Chart;
using UnityEngine;

namespace RetroRecord.Library.Crates
{
    public class AlbumObject : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer m_meshRenderer;

        private Album m_album;

        public void Load(Album album)
        {
            m_album = album;
            if (m_album.Cover != null)
                m_meshRenderer.material.mainTexture = m_album.Cover.texture;
            else
                Debug.LogWarning($"{m_album.AlbumName} doesn't have a cover texture");
        }
    }
}