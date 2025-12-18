using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Zlipacket.CoreZlipacket.Tools;

namespace Zlipacket.VNZlipacket.GraphicPanel
{
    public class GraphicObject
    {
        public const string NAME_FORMAT = "Graphic - [{0}]";
        public const string MATERIAL_PATH = "Materials/layerTransitionMaterial";
        public const string MATERIAL_FIELD_COLOR = "_Color";
        public const string MATERIAL_FIELD_MAINTEX = "_MainTex";
        public const string MATERIAL_FIELD_BLENDTEX = "_BlendTex";
        public const string MATERIAL_FIELD_BLEND = "_Blend";
        public const string MATERIAL_FIELD_ALPHA = "_Alpha";
        public RawImage renderer;

        private GraphicLayer layer;
        
        public bool isVideo
        {
            get { return video != null;  }
        }
        public VideoPlayer video;
        public AudioSource audio;

        public string graphicPath = "";
        public string graphicName { get; private set; }

        private Coroutine co_FadeIn = null;
        private Coroutine co_FadeOut = null;

        public GraphicObject(GraphicLayer layer, string graphicPath, Texture tex)
        {
            this.graphicPath = graphicPath;
            this.layer = layer;
            
            GameObject ob = new GameObject();
            ob.transform.SetParent(layer.panel);
            renderer = ob.AddComponent<RawImage>();
            
            graphicName = tex.name;
            renderer.name = string.Format(NAME_FORMAT, graphicName);
            
            InitGraphic();
            
            renderer.name = string.Format(NAME_FORMAT, graphicName);
            renderer.material.SetTexture(MATERIAL_FIELD_MAINTEX, tex);
        }
        
        public GraphicObject(GraphicLayer layer, string graphicPath, VideoClip clip, bool useAudio)
        {
            this.graphicPath = graphicPath;
            
            GameObject ob = new GameObject();
            ob.transform.SetParent(layer.panel);
            renderer = ob.AddComponent<RawImage>();
            
            graphicName = clip.name;
            renderer.name = string.Format(NAME_FORMAT, graphicName);
            
            InitGraphic();
            
            RenderTexture tex = new RenderTexture(Mathf.RoundToInt(clip.width), Mathf.RoundToInt(clip.height), 0);
            renderer.material.SetTexture(MATERIAL_FIELD_MAINTEX, tex);

            video = renderer.gameObject.AddComponent<VideoPlayer>();
            video.playOnAwake = true;
            video.source = VideoSource.VideoClip;
            video.clip = clip;
            video.renderMode = VideoRenderMode.RenderTexture;
            video.targetTexture = tex;
            video.isLooping = true;
            
            video.audioOutputMode = VideoAudioOutputMode.AudioSource;
            audio = video.gameObject.AddComponent<AudioSource>();

            audio.volume = 0f;
            if (!useAudio)
                audio.mute = true;
            
            video.SetTargetAudioSource(0, audio);
            video.frame = 0;
            video.Prepare();
            video.Play();

            video.enabled = false;
            video.enabled = true;
        }

        private void InitGraphic()
        {
            renderer.transform.localPosition = Vector3.zero;
            renderer.transform.localScale = Vector3.one;
            
            RectTransform rect = renderer.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.one;

            renderer.material = GetTransitionMaterial();
            
            renderer.material.SetFloat(MATERIAL_FIELD_BLEND, 0f);
            renderer.material.SetFloat(MATERIAL_FIELD_ALPHA, 0f);
        }

        private Material GetTransitionMaterial()
        {
            Material mat = Resources.Load<Material>(MATERIAL_PATH);
            
            if (mat != null)
                return new Material(mat);
            
            return null;
        }

        private GraphicPanelsManager panelsManager => GraphicPanelsManager.Instance;
        
        public Coroutine FadeIn(float speed = 3f, Texture blend = null)
        {
            if (co_FadeOut != null)
                panelsManager.StopCoroutine(co_FadeOut);

            if (co_FadeIn != null)
                return co_FadeIn;
            
            co_FadeIn = panelsManager.StartCoroutine(Fading(1f, speed, blend));
            return co_FadeIn;
        }
        
        public Coroutine FadeOut(float speed = 3f, Texture blend = null)
        {
            if (co_FadeIn != null)
                panelsManager.StopCoroutine(co_FadeIn);

            if (co_FadeOut != null)
                return co_FadeOut;
            
            co_FadeOut = panelsManager.StartCoroutine(Fading(0f, speed, blend));
            return co_FadeOut;
        }

        private IEnumerator Fading(float target, float speed, Texture blend = null)
        {
            bool isBlending = blend != null;
            bool fadeIn = target > 0f;
            
            renderer.material.SetTexture(MATERIAL_FIELD_BLENDTEX, (Texture2D)blend);
            renderer.material.SetFloat(MATERIAL_FIELD_ALPHA, isBlending ? 1f : fadeIn ? 0f : 1f);
            renderer.material.SetFloat(MATERIAL_FIELD_BLEND, isBlending ? fadeIn ? 0f : 1f : 1f);
            
            string opacityParam = isBlending ? MATERIAL_FIELD_BLEND : MATERIAL_FIELD_ALPHA;
            
            while (!ZlipUtilities.ApproximatelyWithMargin(renderer.material.GetFloat(opacityParam), target, 0.001f))
            {
                float opacity = Mathf.MoveTowards(renderer.material.GetFloat(opacityParam), target, speed * Time.deltaTime);
                renderer.material.SetFloat(opacityParam, opacity);

                if (isVideo)
                    audio.volume = opacity;
                
                yield return null;
            }
            
            co_FadeIn =  null;
            co_FadeOut = null;

            if (target == 0f)
                Destroy();
            else
                DestroyBackgroundGraphicOnLayer();
        }

        private void Destroy()
        {
            if (layer.currentGraphic != null && layer.currentGraphic.renderer == renderer)
                layer.currentGraphic = null;
                
            Object.Destroy(renderer.gameObject);
        }

        private void DestroyBackgroundGraphicOnLayer()
        {
            layer.DestroyOldGraphics();
        }
    }
}