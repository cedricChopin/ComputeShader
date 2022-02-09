using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateParticle : MonoBehaviour
{

    private Vector2 cursorPos;

    [SerializeField]
    private Transform pos;

    public GameObject plane;

    /// <summary>
    /// Mesh de MEUF
    /// </summary>
    [SerializeField]
    private MeshFilter MEUF;

    /// <summary>
    /// Tableau des vertices
    /// </summary>
    private Vector3[] vertices;
    private Vector3[] verticesWorld;

    // struct
    struct Particle
    {
        public Vector3 position;
        public Vector3 velocity;
        public float life;
    }
    Particle[] particleArray;
    /// <summary>
    /// Taille en octet d'une particule
    /// float = 4 bytes
    /// Vector3 = 12 bytes
    /// </summary>
    private const int SIZE_PARTICLE = 28; 

    /// <summary>
    /// Nombre de particules créés 
    /// </summary>
    public int particleCount = 1000000;

    /// <summary>
    /// Puissance de la gravité
    /// </summary>
    public float gravityPower = 5f;

    /// <summary>
    /// Durée de vie d'une particule
    /// </summary>
    public float lifeTime = 4f;

    /// <summary>
    /// Puissance de direction
    /// </summary>
    public float dirPower = 0f;

    /// <summary>
    /// Materiel utilisé pour afficher les particules
    /// </summary>
    public Material material;

    /// <summary>
    /// Compute Shader utilisé pour mettre à jour les particules
    /// </summary>
    public ComputeShader computeShader;

    /// <summary>
    /// Id du kernel utilisé
    /// </summary>
    private int mComputeShaderKernelID;

    /// <summary>
    /// Buffer contenant les particules
    /// </summary>
    private ComputeBuffer particleBuffer;

    /// <summary>
    /// Buffer des outputs de collisions
    /// </summary>
    private ComputeBuffer output;

    /// <summary>
    /// faut un array avant parce que ça marche po....
    /// </summary>
    private float[] outputArray;

    /// <summary>
    /// COmputeuffer pour les vertices de MEUF
    /// </summary>
    private ComputeBuffer verticesBuffer;

    /// <summary>
    /// Tableau de resultats des collisions 
    /// </summary>
    private float[] result;
    
    /// <summary>
    /// Number of particle per warp.
    /// </summary>
    private const int WARP_SIZE = 256; 

    /// <summary>
    /// Number of warp needed.
    /// </summary>
    private int mWarpCount;

    /// <summary>
    /// on initialise car il casse les couilles....
    /// </summary>
    private int handle_init;

    //public ComputeShader shader;

    // Use this for initialization
    void Start()
    {

        InitComputeShader();

    }

    void InitComputeShader()
    {
        mWarpCount = Mathf.CeilToInt((float)particleCount / WARP_SIZE);

        vertices = MEUF.sharedMesh.vertices;
        // initialize the particles
        particleArray = new Particle[particleCount];
        outputArray = new float[1];
        outputArray[0] = 0;
        int indiceVertex = 0;
        verticesWorld = vertices;
        for (int i = 0; i < particleCount; i++)
        {
            float x = Random.value * 2 - 1.0f;
            float y = Random.value * 2 - 1.0f;
            float z = Random.value * 2 - 1.0f;
            Vector3 xyz = new Vector3(x, y, z);
            xyz.Normalize();
            xyz *= Random.value;
            xyz *= 0.5f;

            Vector3 posVertice = MEUF.transform.TransformPoint( vertices[indiceVertex]);
            verticesWorld[indiceVertex] = posVertice;
            //particleArray[i].position.x = xyz.x;
            //particleArray[i].position.y = xyz.y;
            //particleArray[i].position.z = xyz.z + 3;

            particleArray[i].position.x = posVertice.x;
            particleArray[i].position.y = posVertice.y;
            particleArray[i].position.z = posVertice.z;


            particleArray[i].velocity.x = 0;
            particleArray[i].velocity.y = 0;
            particleArray[i].velocity.z = 0;

            // Initial life value
            particleArray[i].life = Random.value * (lifeTime + 1.0f) + 1.0f;
            if(indiceVertex + 1 < vertices.Length)
            {
                indiceVertex++;
            }
            else
            {
                indiceVertex = 0;
            }
        }

        // create compute buffer
        particleBuffer = new ComputeBuffer(particleCount, SIZE_PARTICLE);
        output = new ComputeBuffer(1, 4, ComputeBufferType.Default);
        verticesBuffer = new ComputeBuffer(vertices.Length, 12, ComputeBufferType.Default);

        particleBuffer.SetData(particleArray);
        output.SetData(outputArray);
        verticesBuffer.SetData(verticesWorld);

        // find the id of the kernel
        //handle_init = computeShader.FindKernel("cs_init");
        mComputeShaderKernelID = computeShader.FindKernel("cs_main");

        // bind the compute buffer to the shader and the compute shader
        //computeShader.SetBuffer(handle_init, "output", output);
        computeShader.SetBuffer(mComputeShaderKernelID, "particleBuffer", particleBuffer);
        computeShader.SetBuffer(mComputeShaderKernelID, "output", output);
        computeShader.SetBuffer(mComputeShaderKernelID, "vertices", verticesBuffer);

        material.SetBuffer("particleBuffer", particleBuffer);
        result = new float[1];
    }

    void OnRenderObject()
    {
        material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, 1, particleCount);
    }

    void OnDestroy()
    {
        if (particleBuffer != null)
            particleBuffer.Release();
    }

    // Update is called once per frame
    void Update()
    {
        computeShader.SetVector("scale", plane.transform.TransformPoint(new Vector3(1.0f, 0.0f, 1.0f)));
        computeShader.SetVector("pos", plane.transform.TransformPoint(new Vector3(1.0f,0.0f,0.0f)));
        computeShader.SetVector("rot", plane.transform.TransformPoint(new Vector3(0.0f,0.0f,0.0f)));
        vertices = MEUF.sharedMesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            verticesWorld[i] = MEUF.transform.TransformPoint(vertices[i]);
        }
        float[] mousePosition2D = { cursorPos.x, cursorPos.y };
        Vector3 posWorld = transform.TransformPoint(MEUF.transform.position);
        float[] objPos = { posWorld.x, posWorld.y, posWorld.z };
        verticesBuffer.SetData(verticesWorld);
        // Send datas to the compute shader
        computeShader.SetFloat("gravityPower", gravityPower);
        computeShader.SetFloat("deltaTime", Time.deltaTime);
        computeShader.SetFloat("dirPower", dirPower);
        computeShader.SetFloat("lifeTime", lifeTime);
        computeShader.SetFloat("lenVertices", vertices.Length);
        computeShader.SetFloats("mousePosition", objPos);

        // Update the Particles
        //computeShader.Dispatch(handle_init, mWarpCount,1,1);
        computeShader.Dispatch(mComputeShaderKernelID, mWarpCount, 1, 1);
        output.GetData(result);
    }

    void OnGUI()
    {
        Vector3 p;
        Camera c = Camera.main;
        Event e = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = e.mousePosition.x;
        mousePos.y = c.pixelHeight - e.mousePosition.y;

        p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, c.nearClipPlane + 14));

        cursorPos.x = p.x;
        cursorPos.y = p.y;
        
        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + c.pixelWidth + ":" + c.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + p.ToString("F3"));
        
        GUILayout.EndArea();
        
        
    }
}