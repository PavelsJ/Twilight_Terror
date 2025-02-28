using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace FODMapping
{
    public class FOD_Manager : MonoBehaviour
    {
        [Header("Coroutines / Actions")]
        private Coroutine FOVCoroutine;
        private Coroutine removeAgentsCoroutine;
        
        public event Action OnFogInitialized;
        public bool IsFogInitialized { get; private set; }
        
        private static readonly Vector2 textureSize = new(320, 320);

        [Header("Fog Settings")]
        [SerializeField] private Color fogColor = new(0.1f, 0.1f, 0.1f, 0.7f);
        [SerializeField] private float updateInterval = 0.02f;
        
        [Header("Shader Initialization")]
        [SerializeField] private Shader fogShader;
        [SerializeField] private ComputeShader computeShader;
        
        private Material fogMaterial;
        private RenderTexture fogTexture;

        [Header("Agent Dictionary")]
        public  List<FOD_Agent> agents = new();
        private const int maxAgentCount = 128;
        
        [Header("Buffers")]
        private readonly List<Vector3> agentData = new(maxAgentCount);
        private ComputeBuffer agentsBuffer;

        [Header("References")]
        public Grid_Manager grid;
        private Animator animator;

        //
        private void Awake()
        {
            fogMaterial = new Material(fogShader);
            GetComponent<SpriteRenderer>().material = fogMaterial;
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
            
            fogTexture = new RenderTexture((int)textureSize.x, (int)textureSize.y, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true, 
                filterMode = FilterMode.Point 
            };
            
            fogMaterial.SetTexture("_FogTexture", fogTexture);
            fogMaterial.SetColor("_FogColor", fogColor);
            fogMaterial.SetVector("_TextureSize", textureSize);

            agentsBuffer = new ComputeBuffer(maxAgentCount, sizeof(float) * 3);
            
            // EnableFOV();
        }
        
        //
        public IEnumerator EnableInstantly()
        {
            yield return new WaitForSeconds(0.1f);
            EnableFOV();

            OnFogInitialized?.Invoke();
            IsFogInitialized = true;
        }

        public IEnumerator EnableWithDelay(float delay)
        {
            animator.SetTrigger("FadeIn");
            yield return new WaitForSeconds(delay);
            EnableFOV();

            OnFogInitialized?.Invoke();
            IsFogInitialized = true;
        }

        //
        private void EnableFOV()
        {
            if (FOVCoroutine == null)
                FOVCoroutine = StartCoroutine(UpdateFOV());
        }

        private void DisableFOV()
        {
            if (FOVCoroutine != null)
            {
                StopCoroutine(FOVCoroutine);
                FOVCoroutine = null;
            }
        }

        //
        private IEnumerator UpdateFOV()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(updateInterval);
                UpdateShaderValues();
            }
        }

        private void UpdateShaderValues()
        {
            if(agents.Count < 1) return;
            
            agentData.Clear();

            Vector2 objectPosition = transform.position;
            Vector2 objectScale = transform.lossyScale;
            Vector2 worldMin = objectPosition - (objectScale / 2);
            Vector2 worldSize = objectScale;

            foreach (var agent in agents)
            {
                if (!agent.enabled && agent.isActive) continue;

                Vector2 worldPos = agent.transform.position;
                Vector2 normalizedPos = (worldPos - worldMin) / worldSize;
                float correctedRadius = (agent.sightRange / worldSize.y) * 10;
                
                agentData.Add(new Vector3(normalizedPos.x, normalizedPos.y, correctedRadius));
            }

            fogMaterial.SetInt("_AgentCount", agentData.Count);

            if (agentData.Count > 0)
            {
                agentsBuffer.SetData(agentData);
                
                fogMaterial.SetBuffer("_Agents", agentsBuffer);
            }

            fogMaterial.SetColor("_FogColor", fogColor);
        }
        
        //
        public void FindAllFOVAgents()
        {
            agents.Clear();
            agents.AddRange(FindObjectsOfType<FOD_Agent>());
        }

        public void AddAgent(FOD_Agent agent)
        {
            if (!agents.Contains(agent) && agent.enabled)
            {
                agents.Add(agent);
            }
        }

        public void RemoveAgent(FOD_Agent agent)
        {
            if (agents.Contains(agent))
            {
                agents.Remove(agent);
            }
        }
        
        // Player Death
        public void RemoveAgentsGradually()
        {
            if (removeAgentsCoroutine != null)
            {
                StopCoroutine(removeAgentsCoroutine);
            }
            
            removeAgentsCoroutine = StartCoroutine(RemoveAgentsCoroutine());
        }

        private IEnumerator RemoveAgentsCoroutine(float time = 0.8f)
        {
            foreach (var agent in new List<FOD_Agent>(agents))
            {
                agent.EndAgent(time);
                yield return new WaitForSeconds(time + 0.2f);
            }
            
            DisableFOV();
            Debug.Log("Game Over");
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        public IEnumerator DisableWithDelay(float delay)
        {
            if (removeAgentsCoroutine != null)
            {
                StopCoroutine(removeAgentsCoroutine);
            }
            
            foreach (var agent in agents)
            {
                agent.EndAgent(0.4f);
            }
            
            yield return new WaitForSeconds(0.5f);

            if (grid != null)
            {
                grid.ChangeGridState();
            }
            
            DisableFOV();
            animator.SetTrigger("FadeOut");
            
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }
        
        private void OnDestroy()
        {
            agentsBuffer?.Release();
        }
    }
}