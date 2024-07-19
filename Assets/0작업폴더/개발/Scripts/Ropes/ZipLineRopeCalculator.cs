using Obi;
using UnityEngine;

public class ZipLineRopeCalculator : MonoBehaviour
{
    private ObiRope _rope;

    [Header("Stop moving if X-distance between currentParticle and first/lastParticle exceeds this value")]
    [SerializeField] private float _stopMargin = 2f;

    private Vector2 _currentParticlePos;
    private Vector2 _firstParticlePos, /*_middleParticlePos,*/ _lastParticlePos;

    private void Awake()
    {
        _rope = gameObject.GetComponent<ObiRope>();
    }

    private void Start()
    {
        GetEndParticlePositions();
    }

    private void GetEndParticlePositions()
    {
        int firstParticle = _rope.elements[0].particle1;
        int lastParticle = _rope.elements[_rope.elements.Count - 1].particle2;
        //int middleParticle = _rope.elements[getParticleWithIndex((_rope.elements.Count - 1) / 2)].particle2;

        _firstParticlePos = getGlobalParticlePos(_rope.solver.positions[firstParticle]);
        //_middleParticlePos = getGlobalParticlePos(_rope.solver.positions[middleParticle]);
        _lastParticlePos = getGlobalParticlePos(_rope.solver.positions[lastParticle]);
    }

    private void OnEnable()
    {
        _rope.solver.OnCollision += Solver_OnCollision;
    }

    private void OnDisable()
    {
        _rope.solver.OnCollision += Solver_OnCollision;
    }

    public Vector2 GetFurtherRopeDir()
    {
        if (_currentParticlePos.x - _firstParticlePos.x < _lastParticlePos.x - _currentParticlePos.x)
        {
            Vector2 diff = _lastParticlePos - _currentParticlePos;
            return diff.normalized;
        }
        else
        {
            Vector2 diff = _firstParticlePos - _currentParticlePos;
            return diff.normalized;
        }
    }

    // dir < 0: left
    // dir > 0: right
    public Vector2 GetNextPulleyDir(float dir)
    {
        if (dir < 0)
        {
            if (_currentParticlePos.x - _firstParticlePos.x < _stopMargin) return Vector2.zero;
            Vector2 diff = _firstParticlePos - _currentParticlePos;
            return diff.normalized;
        }
        else if (dir > 0)
        {
            if (_lastParticlePos.x - _currentParticlePos.x < _stopMargin) return Vector2.zero;
            Vector2 diff = _lastParticlePos - _currentParticlePos;
            return diff.normalized;
        }
        else
        {
            return Vector2.zero;
        }
    }

    private int getIndexInActor(int particle)
    {
        return _rope.solver.particleToActor[particle].indexInActor;
    }

    private int getParticleWithIndex(int indexInActor)
    {
        return _rope.solverIndices[indexInActor];
    }

    private Vector3 getGlobalParticlePos(Vector3 particlePosition)
    {
        Vector3 childUpdated = transform.parent.rotation * Vector3.Scale(particlePosition, transform.parent.lossyScale);

        return childUpdated + transform.parent.position;
    }

    private void Solver_OnCollision(object sender, ObiSolver.ObiCollisionEventArgs e)
    {
        var world = ObiColliderWorld.GetInstance();
        foreach (var contact in e.contacts)
        {
            if (contact.distance < 0.01)
            {
                /* do collsion of bodyB */
                var col = world.colliderHandles[contact.bodyB].owner;

                if (col != null && col.name.Equals("ZipLine Pulley"))
                {
                    /* do collsion of bodyA particles */
                    _currentParticlePos = getGlobalParticlePos(contact.pointB);

                    break;
                }
            }
        }
    }

}
