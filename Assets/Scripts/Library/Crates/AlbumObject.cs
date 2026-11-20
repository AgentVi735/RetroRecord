using RetroRecord.Engine.Chart;
using UnityEngine;

namespace RetroRecord.Library.Crates
{
    public class AlbumObject : CrateObject
    {
        public override CrateObjectType Type => CrateObjectType.Album;
        
        [SerializeField]
        private MeshRenderer m_meshRenderer;
        
        public Album Album { get; private set; }

        public void Load(Album album, bool startFlipped)
        {
            Album = album;
            transform.localRotation = Quaternion.Euler(startFlipped ? m_settings.FlippedRotation : m_settings.DefaultRotation);
            if (Album.Cover != null)
                m_meshRenderer.material.mainTexture = Album.Cover.texture;
            else
                Debug.LogWarning($"{Album.AlbumName} doesn't have a cover texture");
        }

        public CameraPosition GetPositon()
        {
            CameraPosition data = new()
            {
                position = transform.position
            };
            return data;
        }
    }
}