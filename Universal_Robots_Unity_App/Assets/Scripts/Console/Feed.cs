using UnityEngine;

/// <summary>
/// Feed subscribes to other things and outputs them into the chat.
/// </summary>
public class Feed : MonoBehaviour
{
    private void OnEnable()
    {
        Robot.CMD.OnSend += OnConnectionDashboard;
        Robot.Connection.OnFeedback += OnConnectionFeedback;
    }

    private void OnDisable()
    {
        Robot.CMD.OnSend -= OnConnectionDashboard;
        Robot.Connection.OnFeedback -= OnConnectionFeedback;
    }

    private string lastFeedback;
    private void OnConnectionFeedback(string feed)
    {
        if (feed == lastFeedback)
        {
            Chat.Show();
            Chat.Hide();
        }
        else
        {
            Chat.SendLocalResponse("Cobot", feed);
            lastFeedback = feed;
        }
    }

    private void OnConnectionDashboard(string feed)
    {
        Chat.SendLocalResponse("UR Dashboard", feed);
    }
}
