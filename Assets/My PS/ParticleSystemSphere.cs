using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemSphere : MonoBehaviour
{

    public struct MParticle
    {
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 direction;

        public float mass;
        public float lifetime;
        public float radius; 
    }


    public Mesh instanceMesh;
    public Material instanceMaterial;
    public ComputeShader computeShader;



    public int particleCount = 1;
    public float particleLifeTime = 6;
    public Vector3 externalForce;
    public static float gravity = -9.8f;
    public Vector3 force = Vector3.zero;
    public Transform emiterPosition;
    public float height =10.0f;
    public float spread=1.5f;
    Vector3 GravityVector = new Vector3(0.0f, gravity, 0.0f);


    public Bounds worldBounds = new Bounds(new Vector3(0, 1, 0), new Vector3(20, 0, 20));
    public float coeffOfRestitution = 0.7f;
    public float coeffOfRepulsion = 1;

    //object to collider
    public GameObject plane;
    //public GameObject Cube;
    //public GameObject Sphere;
    //public GameObject plane;
    //public Mesh mesh;

    //public MeshCollider meshCollider;
    //Vector3[] verticesPlane;
    //

    private int cachedInstanceCount = -1;
    private int cachedSubMeshIndex = -1;

    private ComputeBuffer ParticleBuffer;
    

    private ComputeBuffer ParticlePropsBuffer;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    private int subMeshIndex = 0;
    private MParticle[] particleArray;
    
    private MParticle[] getParticleArrayFromBuffer = new MParticle[1];



    //colider 
   // public List<Collider> colliderList;
    // Start is called before the first frame update
    void Start()
    {

        

        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        UpdateBuffers();
        //verticesPlane = plane.GetComponent<MeshFilter>().sharedMesh.vertices;
        //Debug.Log("vertex : "+mesh.vertexCount);
        //Debug.Log("triangle : "+ mesh.triangles.Length);
        //Debug.Log("Clothes Point : "+ meshCollider.ClosestPoint(new Vector3(0,-5,0)));
    }

    // Update is called once per frame
    void Update()
    {

        
        // sent data to compute shader
        if (cachedInstanceCount != particleCount || cachedSubMeshIndex != subMeshIndex) UpdateBuffers();
        //UpdateBuffers();



        computeShader.SetFloat("dt", Time.deltaTime);
        computeShader.SetVector("ExternalForce", externalForce);
        computeShader.SetInt("ParticleCount", particleCount);
        computeShader.SetVector("Gravity", GravityVector);
        computeShader.SetVector("emitterposition", emiterPosition.position);
        computeShader.SetFloat("particleLifeTime", particleLifeTime);
        computeShader.SetFloat("coeffOfRestitution", coeffOfRestitution);
        computeShader.SetFloat("coeffOfRepulsion", coeffOfRepulsion);
        
        computeShader.SetBuffer(0, "ParticleBuffer", ParticleBuffer);
        computeShader.Dispatch(0, particleCount / 64 + 1, 1, 1);

        instanceMaterial.SetBuffer("ParticleBuffer", ParticleBuffer);
        instanceMaterial.SetBuffer("ParticlePropsBuffer", ParticlePropsBuffer);
        //if(Input.GetKeyDown(KeyCode.Space))
         Graphics.DrawMeshInstancedIndirect(instanceMesh, subMeshIndex, instanceMaterial, worldBounds, argsBuffer);
        ParticleBuffer.GetData(getParticleArrayFromBuffer);

       // Graphics.Draw

        //UpdateBuffers();
    }
    void UpdateBuffers()
    {
        if (instanceMesh != null)
            subMeshIndex = Mathf.Clamp(subMeshIndex, 0, instanceMesh.subMeshCount - 1);
        if (ParticleBuffer != null)
            ParticleBuffer.Release();

        ParticleBuffer = new ComputeBuffer(particleCount, 12 * sizeof(float)); // create particle buffer 
        particleArray = new MParticle[particleCount];

        for (int i = 0; i < particleCount; i++)
        {
            particleArray[i].position = emiterPosition.position;
            particleArray[i].velocity = Vector3.zero;
            //Vector3 randomDir = new Vector3((Random.Range(-1000.0f, 1000.0f) / 1000.0f),
            //                                (Random.Range(-1000.0f, 1000.0f) / 1000.0f),
            //                                (Random.Range(-1000.0f, 1000.0f) / 1000.0f));
            Vector3 randomDir = Random.insideUnitSphere;

            particleArray[i].velocity = new Vector3(0.0f, height, 0.0f) + (randomDir * spread);
            particleArray[i].direction = new Vector3(0.0f, height, 0.0f) + (randomDir * spread);
            particleArray[i].lifetime = Random.Range(-1.0f,particleLifeTime);
            particleArray[i].radius = 0.25f;//Random.Range(0.05f, 0.5f);
            particleArray[i].mass = 1.0f; //mass base on volume = ((4.0f / 3.0f) * 3.14 * pow(radius, 3));

        }
        ParticleBuffer.SetData(particleArray);
        instanceMaterial.SetBuffer("ParticleBuffer", ParticleBuffer);
        if (ParticlePropsBuffer != null) ParticlePropsBuffer.Release();

        ParticlePropsBuffer = new ComputeBuffer(particleCount, 4 * sizeof(float));
        Vector4[] props = new Vector4[particleCount];
        for (int i = 0; i < particleCount; i++)
        {
            props[i].x = 0.9f;
            props[i].y = 0.5f;
            props[i].z = 0.9f;
            props[i].w = 0.0f;
            //props[i].x = 0.5f + 0.5f * Random.value;
            //props[i].y = 0.5f + 0.5f * Random.value;
            //props[i].z = 0.5f + 0.5f * Random.value;
            //props[i].w = Random.value;
        }

        ParticlePropsBuffer.SetData(props);
        instanceMaterial.SetBuffer("ParticlePropsBuffer", ParticlePropsBuffer);
        if (instanceMesh != null)
        {
            args[0] = (uint)instanceMesh.GetIndexCount(subMeshIndex);
            args[1] = (uint)particleCount;
            args[2] = (uint)instanceMesh.GetIndexStart(subMeshIndex);
            args[3] = (uint)instanceMesh.GetBaseVertex(subMeshIndex);
        }
        else
        {
            args[0] = args[1] = args[2] = args[3] = 0;
        }
        argsBuffer.SetData(args);
        cachedInstanceCount = particleCount;
        cachedSubMeshIndex = subMeshIndex;
        //Debug.Log("asdaqsdasd");
    }


    void OnGUI()
    {
        GUIStyle gui_style = new GUIStyle();
        gui_style.fontSize = 25;
        GUI.Label(new Rect(265, 20, 200, 30), "Instance Count: " + particleCount.ToString(),gui_style);
        GUI.Label(new Rect(25, 60, 200, 30), "fps: "+(1/Time.smoothDeltaTime), gui_style);
        
        particleCount = (int)GUI.HorizontalSlider(new Rect(25, 20, 200, 30), (float)particleCount, 1.0f, 50000.0f);
    }
    void OnDisable()
    {
        ParticleBuffer.Release();
        ParticlePropsBuffer.Release();
        argsBuffer.Release();
    }
}
