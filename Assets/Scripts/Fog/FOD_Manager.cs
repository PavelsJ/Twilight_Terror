using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace FODMapping
{
    public class FOD_Manager : MonoBehaviour
    {
        private Coroutine FOVCoroutine;
        private Coroutine removeAgentsCoroutine;
        
        private static readonly Vector2 textureSize = new(320, 320);

        [SerializeField] private Color fogColor = new(0.1f, 0.1f, 0.1f, 0.7f);
        [SerializeField] private float updateInterval = 0.02f;

        public event Action OnFogInitialized;
        public bool IsFogInitialized { get; private set; }

        [SerializeField] private Shader fogShader;
        private Material fogMaterial;
        private RenderTexture fogTexture;

        private readonly List<FOD_Agent> agents = new();
        private const int maxAgentCount = 128;

        private readonly List<Vector3> agentData = new(maxAgentCount);
        private ComputeBuffer agentsBuffer;
        
        private readonly List<float> transparencies = new(maxAgentCount);
        private ComputeBuffer transparencyBuffer;

        public Grid_Manager grid;

        private void Awake()
        {
            fogMaterial = new Material(fogShader);
            GetComponent<SpriteRenderer>().material = fogMaterial;
        }

        private void Start()
        {
            fogTexture = new RenderTexture((int)textureSize.x, (int)textureSize.y, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true, 
                filterMode = FilterMode.Point 
            };
            
            fogMaterial.SetTexture("_FogTexture", fogTexture);
            fogMaterial.SetColor("_FogColor", fogColor);
            fogMaterial.SetVector("_TextureSize", textureSize);

            agentsBuffer = new ComputeBuffer(maxAgentCount, sizeof(float) * 3);
            transparencyBuffer = new ComputeBuffer(maxAgentCount, sizeof(float));
            
            // EnableFOV(); // Включение обновления тумана по умолчанию
        }

        public IEnumerator EnableWithDelay(float delay)
        {
            GetComponent<Animator>().SetTrigger("FadeIn");
            yield return new WaitForSeconds(delay);
            EnableFOV();

            OnFogInitialized?.Invoke();
            IsFogInitialized = true;
        }

        private void OnDestroy()
        {
            agentsBuffer?.Release();
            transparencyBuffer?.Release();
        }

        public void EnableFOV()
        {
            if (FOVCoroutine == null)
                FOVCoroutine = StartCoroutine(UpdateFOV());
        }

        public void DisableFOV()
        {
            if (FOVCoroutine != null)
            {
                StopCoroutine(FOVCoroutine);
                FOVCoroutine = null;
            }
        }

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
            agentData.Clear();
            transparencies.Clear();

            Vector2 objectPosition = transform.position;
            Vector2 objectScale = transform.lossyScale;
            Vector2 worldMin = objectPosition - (objectScale / 2);
            Vector2 worldSize = objectScale;

            foreach (var agent in agents)
            {
                if (!agent.enabled || !agent.contributeToFOV) continue;

                Vector2 worldPos = agent.transform.position;
                Vector2 normalizedPos = (worldPos - worldMin) / worldSize;
                float correctedRadius = (agent.sightRange / worldSize.y) * 10;
                
                agentData.Add(new Vector3(normalizedPos.x, normalizedPos.y, correctedRadius));
                transparencies.Add(agent.sightTransparency);
            }

            fogMaterial.SetInt("_AgentCount", agentData.Count);

            if (agentData.Count > 0)
            {
                agentsBuffer.SetData(agentData);
                transparencyBuffer.SetData(transparencies);
                
                fogMaterial.SetBuffer("_Agents", agentsBuffer);
                fogMaterial.SetBuffer("_Transparency", transparencyBuffer);
            }

            fogMaterial.SetColor("_FogColor", fogColor);
        }

        public void FindAllFOVAgents()
        {
            agents.Clear();
            agents.AddRange(FindObjectsOfType<FOD_Agent>());
        }

        public void AddAgent(FOD_Agent agent)
        {
            if (!agents.Contains(agent))
                agents.Add(agent);
        }

        public void RemoveAgent(FOD_Agent agent)
        {
            if (agents.Contains(agent))
                agents.Remove(agent);
        }
        
        public void RemoveAgentsGradually()
        {
            if (removeAgentsCoroutine != null)
            {
                StopCoroutine(removeAgentsCoroutine);
            }
            
            removeAgentsCoroutine = StartCoroutine(RemoveAgentsCoroutine());
        }

        private IEnumerator RemoveAgentsCoroutine(float time = 1.1f)
        {
            while (agents.Count > 0)
            {
                FOD_Agent agent = agents[0];
                agent.EndAgent();
                yield return new WaitForSeconds(time);
            }
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
                grid.ChangePlayerPos();
            }
            
            DisableFOV();
            GetComponent<Animator>().SetTrigger("FadeOut");
            
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }
    }
}