using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class panel : MonoBehaviour
{
    public void clickedAgain()
    {
        this.gameObject.SetActive(false);
    }

    public void Death()
    {
        this.gameObject.SetActive(true);
    }

}
