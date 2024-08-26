using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBXAvatarLoader : MonoBehaviour
{

    [Header("Avatar Settings")]
    public GameObject CurrentAvatar;
    public bool OnStartChange;

    [Header("Parent Settings")]
    public GameObject TargetParent;
    public GameObject TargetReference;
    public bool isLookAtPlayer;
    public bool isNPC;
    GameObject[] players;

    public void ChangePrefabs(GameObject NewPrefabs)
    {
        if (NewPrefabs != null)
        {
            CurrentAvatar = GameObject.Instantiate(NewPrefabs);
        }
        CurrentAvatar.transform.parent = TargetParent.transform;
        CurrentAvatar.transform.position = TargetReference.transform.position;
        CurrentAvatar.transform.rotation = TargetReference.transform.rotation;
        HideAllCharacter();
        CurrentAvatar.SetActive(true);
        if (!isNPC) GetComponent<FBXMovementController>().Init();
    }

    public void ChangeCharacter(GameObject NewAvatar)
    {
        if (NewAvatar != null)
        {
            CurrentAvatar = NewAvatar;
        }
        CurrentAvatar.transform.parent = TargetParent.transform;
        CurrentAvatar.transform.position = TargetReference.transform.position;
        CurrentAvatar.transform.rotation = TargetReference.transform.rotation;
        HideAllCharacter();
        CurrentAvatar.SetActive(true);
        if (!isNPC) GetComponent<FBXMovementController>().Init();
    }

    public void HideAllCharacter()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            // Mendapatkan child ke-i dari transform
            Transform child = transform.GetChild(i);

            // Mengecek apakah child tersebut memiliki komponen Animator
            Animator animator = child.GetComponent<Animator>();
            if (animator != null)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (OnStartChange)
        {
            ChangeCharacter(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isNPC && isLookAtPlayer)
        {
            LookAtPlayer();
        }
    }

    void LookAtPlayer()
    {
        // Mencari GameObject dengan tag "Player"
        if (players == null)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }

        // Looping untuk setiap GameObject dengan tag "Player"
        foreach (GameObject player in players)
        {
            // Mengecek apakah GameObject dengan tag "Player" merupakan pemilik tag
            if (player.GetComponent<FBXAvatarLoader>() != null)
            {
                // Mengubah arah GameObject agar menghadap ke pemilik tag "Player"
                transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
            }
        }
    }
}
