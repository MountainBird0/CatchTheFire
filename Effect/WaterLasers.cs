using DG.Tweening.Core.Easing;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class WaterLasers : NetworkBehaviour
{
    public float laserScale = 1;
    public Color laserColor = new Vector4(1, 1, 1, 1);
    public GameObject hitEffect;
    public GameObject flashEffect;
    public float hitOffset = 0;

    //public float maxLength;
    public NetworkVariable<float> maxLength = new(0.0f);
    private float clientMaxLength;

    public LayerMask layerMask;

    public bool UpdateSaver = false;
    private ParticleSystem laserPS;
    private ParticleSystem[] falshParticle;
    private ParticleSystem[] hitParticle;
    private Material laserMat;
    private int particleCount;
    private ParticleSystem.Particle[] particles;
    private Vector3[] particlePositions;
    public float dissovleTimer = 0;
    public bool startDissovle = false;
    private Vector3[] bazierPos;

    Vector3 point0;
    Vector3 point1;
    Vector3 point2;

    //public NetworkVariable<bool> startDissovle = new(false);

    private void Awake()
    {
        laserPS = GetComponent<ParticleSystem>();
        laserMat = GetComponent<ParticleSystemRenderer>().material;
        falshParticle = flashEffect.GetComponentsInChildren<ParticleSystem>();
        hitParticle = hitEffect.GetComponentsInChildren<ParticleSystem>();
        laserMat.SetFloat("_Scale", laserScale);

        maxLength.OnValueChanged += OnChangeMaxValue;
    }

    private void Update()
    {
        //if (startDissovle)
        //{
        //    dissovleTimer += Time.deltaTime;
        //    laserMat.SetFloat("_Dissolve", dissovleTimer * 5);
        //}
    }
    public void FireLaser()
    {
        if(UpdateSaver == false)
        {
            //laserMat.SetFloat("_Dissolve", 0);

            //laserMat.SetVector("_StartPoint", transform.position);
            //var EndPos = new Vector3(100, 100, 100);
            //laserMat.SetFloat("_Distance", Vector3.Distance(transform.position, EndPos));
            //laserMat.SetVector("_EndPoint", EndPos);

            if (hitParticle != null)
            {
                //hitEffect.transform.position = EndPos;
                foreach (var AllHits in hitParticle)
                {
                    if (!AllHits.isPlaying) AllHits.Play();
                }
                foreach (var AllFlashes in falshParticle)
                {
                    if (!AllFlashes.isPlaying) AllFlashes.Play();
                }
            }
        }
    }

    /* //Bazeir Curve Code

    public void FireLaser()
    {
        if (laserPS != null && UpdateSaver == false)
        {
            laserMat.SetFloat("_Dissolve", 0);

            laserMat.SetVector("_StartPoint", transform.position);

            CurveWaterLaser(clientMaxLength); //Calculate Curve

            var EndPos = bazierPos[bazierPos.Length - 1];

            laserMat.SetFloat("_Distance", Vector3.Distance(transform.position, EndPos));
            laserMat.SetVector("_EndPoint", EndPos);

            AddParticles();


            if (hitParticle != null)
            {
                hitEffect.transform.position = EndPos;
                foreach (var AllHits in hitParticle)
                {
                    if (!AllHits.isPlaying) AllHits.Play();
                }
                foreach (var AllFlashes in falshParticle)
                {
                    if (!AllFlashes.isPlaying) AllFlashes.Play();
                }
            }

        }
    }*/

    /* //Save Script
    public void FireLaser()
    {
        if (laserPS != null && UpdateSaver == false)
        {
            laserMat.SetFloat("_Dissolve", 0);

            laserMat.SetVector("_StartPoint", transform.position);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, clientMaxLength, layerMask))
            {
                particleCount = Mathf.RoundToInt(hit.distance / (2 * laserScale));
                if (particleCount < hit.distance / (2 * laserScale))
                {
                    particleCount += 1;
                }
                particlesPositions = new Vector3[particleCount];
                AddParticles();

                laserMat.SetFloat("_Distance", hit.distance);
                laserMat.SetVector("_EndPoint", hit.point);
                if (hitParticle != null)
                {
                    hitEffect.transform.position = hit.point + hit.normal * hitOffset;
                    hitEffect.transform.LookAt(hit.point);
                    foreach (var AllHits in hitParticle)
                    {
                        if (!AllHits.isPlaying) AllHits.Play();
                    }
                    foreach (var AllFlashes in falshParticle)
                    {
                        if (!AllFlashes.isPlaying) AllFlashes.Play();
                    }
                }
            }
            else
            {
                var EndPos = transform.position + transform.forward * clientMaxLength;
                var distance = Vector3.Distance(EndPos, transform.position);
                particleCount = Mathf.RoundToInt(distance / (2 * laserScale));
                if (particleCount < distance / (2 * laserScale))
                {
                    particleCount += 1;
                }
                particlesPositions = new Vector3[particleCount];
                AddParticles();

                laserMat.SetFloat("_Distance", distance);
                laserMat.SetVector("_EndPoint", EndPos);
                if (hitParticle != null)
                {
                    hitEffect.transform.position = EndPos;
                    foreach (var AllPs in hitParticle)
                    {
                        if (AllPs.isPlaying) AllPs.Stop();
                    }
                    foreach (var AllFlashes in falshParticle)
                    {
                        if (!AllFlashes.isPlaying) AllFlashes.Play();
                    }
                }
            }
        }
    }
    */


    private void CurveWaterLaser(float rayMaxValue)
    {
        int i = 0;
        bazierPos = new Vector3[11];
        RaycastHit hitInfo;
        Vector3 thisPosition = transform.position;

        Vector3 endPoint = transform.position + transform.forward * rayMaxValue;

        if(Physics.Raycast(endPoint, Vector3.down, out hitInfo, Mathf.Infinity, layerMask))
        {
            point0 = thisPosition;
            point1 = endPoint;
            point1 = (point0 + point1) * 0.5f;
            point2 = hitInfo.point;


            for (float t = 0; t <= 1; t += 0.1f)
            {
                Vector3 positionOnCurve = CalculateQuadraticBezier(point0, point1, point2, t);
                bazierPos[i++] = positionOnCurve;
            }
        }
    }

    Vector3 CalculateQuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return (1 - t) * (1 - t) * p0 + 2 * t * p1 * (1 - t) + t * t * p2;
    }


    private void AddParticles()
    {
        if (bazierPos == null || bazierPos.Length == 0) return;

        particleCount = bazierPos.Length;
        particles = new ParticleSystem.Particle[particleCount];
        particlePositions = new Vector3[particleCount];

        for (int i = 0; i < particleCount; i++)
        {
            //particlesPositions[i] = new Vector3(0f, 0f, 0f) + new Vector3(0f, 0f, i * 2 * laserScale);
            particlePositions[i] = bazierPos[i] - transform.position;
            particles[i].position = particlePositions[i];
            particles[i].startSize3D = new Vector3(0.001f, 0.001f, 2 * laserScale);
            particles[i].startColor = laserColor;
        }
        laserPS.SetParticles(particles, particles.Length);
    }
    public void DisablePrepare()
    {
        UpdateSaver = true;
        dissovleTimer = 0;
        startDissovle = true;
        if (falshParticle != null && hitParticle != null)
        {
            //laserPS.Stop();
            //Debug.Log("Particle Stop!");
            foreach (var AllHits in hitParticle)
            {
                if (AllHits.isPlaying) AllHits.Stop();
            }
            foreach (var AllFlashes in falshParticle)
            {
                if (AllFlashes.isPlaying) AllFlashes.Stop();
            }
        }
    }

    private void OnChangeMaxValue(float previous, float current)
    {
        clientMaxLength = current;
    }

}
