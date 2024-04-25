using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
 
 
public class PlayerBT : BehaviorTree.Tree
{
    public UnityEngine.Transform[] waypoints;
 
 
    public static float speed = 2f;
    public static float fovRange = 6f;
    public static float attackRange = 1.5f;
 
 
    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckEnemyInFOVRange(transform),
                new TaskGoToTarget(transform),
                new CheckEnemyInAttackRange(transform)
            }),
            new TaskPatrol(transform, waypoints),
        });
 
 
        return root;
    }
}
 
 
public class CheckEnemyInAttackRange : Node
{
    private Transform _transform;
    private Animator _animator;
 
 
    public CheckEnemyInAttackRange(Transform transform)
    {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
    }
 
 
    public override NodeState Evaluate()
    {
        Debug.Log("?");
        object t = GetData("target");
        if (t == null)
        {
            state = NodeState.FAILURE;
            return state;
        }
        Transform target = (Transform)t;
        Debug.Log("Distance: " + Vector3.Distance(_transform.position, target.position));
        if (Vector3.Distance(_transform.position, target.position) <= PlayerBT.attackRange)
        {
            Debug.Log("Kick its butt!");
            _animator.SetBool("IsAttacking", true);
            _animator.SetBool("IsMoving", false);
            state = NodeState.SUCCESS;
            return state;
        }
 
 
        state = NodeState.FAILURE;
        return state;
    }
 
 
}
 
 
public class CheckEnemyInFOVRange : Node
{
    private static int _enemyLayerMask = 1 << 6;
    private Transform _transform;
    private Animator _animator;
 
 
    public CheckEnemyInFOVRange(Transform transform)
    {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
    }
 
 
    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null)
        {
            Collider[] colliders = Physics.OverlapSphere(
                _transform.position, PlayerBT.fovRange, _enemyLayerMask);
 
 
            if (colliders.Length > 0)
            {
                Debug.Log("Enemy nearby...");
                parent.parent.SetData("target", colliders[0].transform);
                _animator.SetBool("IsMoving", true);
                state = NodeState.SUCCESS;
                return state;
            }
 
 
            state = NodeState.FAILURE;
            return state;
        }
 
 
        Debug.Log("Enemy nearby...");
        state = NodeState.SUCCESS;
        return state;
    }
 
 
}
 
 
public class TaskGoToTarget : Node
{
    private Transform _transform;
    public TaskGoToTarget(Transform transform)
    {
        _transform = transform;
    }
 
 
    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        Debug.Log("Distance (ttt): " + Vector3.Distance(_transform.position, target.position));
        if (Vector3.Distance(_transform.position, target.position) > 0.1f)
        {
            _transform.position = Vector3.MoveTowards(
                _transform.position, target.position, PlayerBT.speed * Time.deltaTime);
            _transform.LookAt(target.position);
                    state = NodeState.RUNNING;
            return state;
        }
        Debug.Log("Close now!");
        state = NodeState.SUCCESS;
        return state;
 
 
    }
 
 
}
 
 
public class TaskPatrol : Node
{
    private Transform _transform;
    private Animator _animator;
    private Transform[] _waypoints;
    private int _currentWaypointIndex = 0;
    private float _waitTime = 1f; // in seconds
    private float _waitCounter = 0f;
    private bool _waiting = false;
 
 
    public TaskPatrol(Transform transform, Transform[] waypoints)
    {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
        _waypoints = waypoints;
    }
 
 
    public override NodeState Evaluate()
    {
        if (_waiting)
        {
            _waitCounter += Time.deltaTime;
            if (_waitCounter >= _waitTime)
            {
                _waiting = false;
                _animator.SetBool("IsMoving", true);
            }
        }
        else
        {
            Transform wp = _waypoints[_currentWaypointIndex];
            if (Vector3.Distance(_transform.position, wp.position) < 0.01f)
            {
                _transform.position = wp.position;
                _waitCounter = 0f;
                _waiting = true;
 
 
                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
                _animator.SetBool("IsMoving", false);
            }
            else
            {
                _transform.position = Vector3.MoveTowards(_transform.position, wp.position, PlayerBT.speed * Time.deltaTime);
                _transform.LookAt(wp.position);
            }
        }
        state = NodeState.RUNNING;
        return state;
    }
}
/**
 * From https://github.com/MinaPecheux/UnityTutorials-BehaviourTrees
 *
 * Winter 2024
 */
 
 
namespace BehaviorTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }
 
 
    public class Node
    {
        protected NodeState state;
 
 
        public Node parent;
        protected List<Node> children = new List<Node>();
 
 
        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();
 
 
        public Node()
        {
            parent = null;
        }
 
 
        public Node(List<Node> children)
        {
            foreach (Node child in children)
                _Attach(child);
        }
 
 
        private void _Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }
 
 
        public virtual NodeState Evaluate() => NodeState.FAILURE;
 
 
        public void SetData(string key, object value)
        {
            _dataContext[key] = value;
        }
 
 
        public object GetData(string key)
        {
            object value = null;
            if (_dataContext.TryGetValue(key, out value))
                return value;
 
 
            Node node = parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null)
                    return value;
                node = node.parent;
            }
            return null;
        }
 
 
        public bool ClearData(string key)
        {
            if (_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }
 
 
            Node node = parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared)
                    return true;
                node = node.parent;
            }
            return false;
        }
    }
 
 
    public class Selector : Node
    {
        public Selector()
            : base() { }
 
 
        public Selector(List<Node> children)
            : base(children) { }
 
 
        public override NodeState Evaluate()
        {
            Debug.Log("Selector");
            foreach (Node node in children)
            {
                Debug.Log("\tEvaluating child " + node);
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    default:
                        continue;
                }
            }
 
 
            state = NodeState.FAILURE;
            return state;
        }
    }
 
 
    public class Sequence : Node
    {
        public Sequence()
            : base() { }
 
 
        public Sequence(List<Node> children)
            : base(children) { }
 
 
        public override NodeState Evaluate()
        {
            Debug.Log("Sequence");
            bool anyChildIsRunning = false;
 
 
            foreach (Node node in children)
            {
                Debug.Log("\tEvalutating child " + node);
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }
 
 
            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }
 
 
    public abstract class Tree : MonoBehaviour
    {
        private Node _root = null;
 
 
        protected void Start()
        {
            _root = SetupTree();
        }
 
 
        private void Update()
        {
            if (_root != null)
                _root.Evaluate();
        }
 
 
        protected abstract Node SetupTree();
    }
}