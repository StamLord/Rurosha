using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Squad
{
    [ReadOnly][SerializeField] private int id = -1; // Negative value means it's uninitialized
    [SerializeField] private List<SquadAgent> squadMembers = new List<SquadAgent>();

    public bool IsInitialized { get { return id >= 0; }}

    // Empty constructor is used by unity automatically when component is added to avoid initializeing before Start()
    public Squad(){}

    // This constructor will be used to initialize purposefully from SquadAgent
    public Squad(int id)
    {
        this.id = id;
    }

    public void AddMember(SquadAgent member)
    {
        if(squadMembers.Contains(member)) return;
        squadMembers.Add(member);
    }

    public void RemoveMember(SquadAgent member)
    {
        squadMembers.Remove(member);
    }

    public void SendMessage(string message, SquadAgent sender = null)
    {
        foreach(SquadAgent agent in squadMembers)
        {
            if(sender == agent) continue;
            agent.GetMessage(message);
        }
    }
}

public class SquadAgent : MonoBehaviour
{
    [Min(0)][SerializeField] private int id;
    [SerializeField] private CharacterStats self;

    [SerializeField] private bool autoJoinSquad;
    [SerializeField] private float squadJoinRange = 10;
    [SerializeField] private bool visualizeJoinRange;

    [SerializeField] private Squad squad = null;

    public int ID { get { return id; }}
    public Squad Squad { get { return squad; }}
    
    public delegate void GetMessageDelegate(string message);
    public event GetMessageDelegate OnGetMessage;

    private void Start()
    {
        if(autoJoinSquad)
            FindSquadMembers();
    }

    /// <summary>
    /// Finds squad agents in a radius and joins / invites them to same squad.
    /// </summary>
    private void FindSquadMembers()
    {
        // Get all coliders in a range
        Collider[] colliders = Physics.OverlapSphere(transform.position, squadJoinRange);

        // Keep track of root transforms we've checked (Many colliders can sit under same root transform)
        List<Transform> checkList = new List<Transform>();

        foreach (Collider col in colliders)
        {
            // We check the root transform where the SquadAgent component should sit
            Transform t = col.transform.root;

            // Don't check same transform twice
            if(checkList.Contains(t)) continue;
            checkList.Add(t);

            SquadAgent other = t.GetComponent<SquadAgent>();

            #region Guard Clauses

            // No SquadAgent component
            if(other == null) continue;

            // Ignore self
            if(other == this) continue;
            
            // Ignore different IDs
            if(other.ID != ID) continue;
            
            #endregion

            // We don't have an initialized squad
            if(squad == null || squad.IsInitialized == false)
            {
                // Both don't have a squad, create squad and invite other to join
                if(other.Squad == null || other.Squad.IsInitialized == false)
                {
                    squad = new Squad(id);
                    squad.AddMember(this);
                    other.Invite(squad);
                }
                // Other has squad, we do not. Join their squad
                else
                    squad = other.JoinSquad(this);
            }
            // Our squad is initialized
            else
            {
                // Other has no squad, inviting to our squad
                if (other.Squad == null || other.Squad.IsInitialized == false)
                    other.Invite(squad);
                // Shouldn't happen
                else if(other.Squad != squad)
                    Debug.LogWarning("Warning: SquadAgents with same ID have different initialized squad objects. " + gameObject.name + ", " + other.gameObject.name);
            }
        }
    }

    /// <summary>
    /// Adds member to squad members and sends back the squad object.
    /// </summary>
    public Squad JoinSquad(SquadAgent member)
    {
        squad.AddMember(member);
        return squad;
    }

    /// <summary>
    /// Joins a squad and adds itself as a member.
    /// </summary>
    public void Invite(Squad squad)
    {
        this.squad = squad;
        this.squad.AddMember(this);
    }

    /// <summary>
    /// Sends a message to it's squad. 
    /// This method is called by the AI states to pass messages like "Low Health" or "Leader Down".
    /// </summary>
    /// <param name="message"></param>
    public new void SendMessage(string message)
    {
        squad?.SendMessage(message, this);
    }

    /// <summary>
    /// Gets a message and fires an event. 
    /// This method is called by the Squad object on all it's members when a message is sent to the squad.
    /// </summary>
    /// <param name="message"></param>
    public void GetMessage(string message)
    {
        if(OnGetMessage != null)
            OnGetMessage(message);
    }

    private void OnDrawGizmos() 
    {
        if(visualizeJoinRange)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, squadJoinRange);
        }
    }
}
