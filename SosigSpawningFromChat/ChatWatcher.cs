using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FistVR;
using BepInEx;
using BepInEx.Configuration;
using System.IO;
using Sodalite;
using Sodalite.Api;
using Sodalite.ModPanel;

using Sodalite.UiWidgets;
using Sodalite.Utilities;
using System.Reflection;

namespace jediSpawner
{


    [BepInPlugin("h3vr.arpy.chatspawner", "ChatSpawner", "1.0.0")]
    public class ChatWatcher : BaseUnityPlugin
    {
        public static ChatWatcher instance;
        public static List<Sosig> spawnedChatters;
        //private static Vector3 followPoint = new Vector3(0, 0, 0);
        private static LayerMask Mask;
        public string filePath = string.Empty;
        private ConfigEntry<string> filePathToTextFolder;
        private ConfigEntry<KeyCode> keyToSpawn;
        private GameObject PrefabToSpawn;
        public string SpawnerName;
        private static readonly string BasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //public string keyName;

        public void Awake()
        {
            filePathToTextFolder = Config.Bind("General",
                "FilePath",
                "null",
                "The File Path to where the name of the chatter can be found");
            keyToSpawn = Config.Bind("General",
                "KeyBind",
                KeyCode.P,
                "The key used to spawn the sosigs h ttps://docs.unity3d.com/Manual/class-InputManager.html");
            var bundle = AssetBundle.LoadFromFile(Path.Combine(BasePath, "JediSpawner"));
            if (bundle)
            {
                PrefabToSpawn = bundle.LoadAsset<GameObject>("Jedit'sSpawner");
            }
            instance = this;
            spawnedChatters = new List<Sosig>();
            Mask = LayerMask.GetMask("Environment");
            
        }


        // Update is called once per frame
        public void Update()
        {
            if (Input.GetKeyDown(keyToSpawn.Value))
            {

                //SpawnerName
                  string str  = File.ReadAllText(filePathToTextFolder.Value);                
                int index = str.IndexOf('"');
                string res = string.Empty;
                for (int i = index; i < str.LastIndexOf('"') - 1; i++)
                    res += str[i + 1];
                SpawnerName = res;
                Debug.Log(str);
                Vector3 spawnPoint = new Vector3(GM.CurrentPlayerBody.Head.transform.position.x, GM.CurrentPlayerBody.transform.position.y, GM.CurrentPlayerBody.Head.transform.position.z + 1);
                GameObject spawner = Instantiate(PrefabToSpawn, spawnPoint, Quaternion.identity);
                //GameObject spawner = Instantiate(PrefabToSpawn, GM.CurrentPlayerBody.Head.transform.position, Quaternion.identity);
                //Rigidbody spawnerBody = (Rigidbody)spawner.GetComponent(typeof(Rigidbody));
                //spawnerBody.AddForce(GM.CurrentPlayerBody.Head.forward, ForceMode.Impulse);
            }
            if (spawnedChatters.Count > 0)
            {




                for (var i = spawnedChatters.Count - 1; i > -1; i--)
                {
                    if (spawnedChatters[i] == null)
                    {
                        spawnedChatters.RemoveAt(i);
                        //Debug.Log("removed a sosig");
                    }
                }
                foreach (Sosig selectedSosig in spawnedChatters)
                {
                    if (!selectedSosig.m_isStunned)
                    {

                        if (Vector3.Distance(GM.CurrentPlayerBody.Head.position, selectedSosig.m_assaultPoint) > 6)
                        {
                            bool isBad = true;
                            
                                float one = ((Random.Range(0, 2) * 2 - 1) * Random.Range(0.75f, 2.5f));
                                float two = ((Random.Range(0, 2) * 2 - 1) * Random.Range(0.75f, 2.5f));
                                Vector3 followPoint = new Vector3(GM.CurrentPlayerBody.Head.position.x + one, GM.CurrentPlayerBody.Head.position.y, GM.CurrentPlayerBody.Head.position.z + two);
                                isBad = Physics.Linecast(GM.CurrentPlayerBody.Head.position, followPoint, Mask);
                                if (!isBad)
                                {
                                    selectedSosig.CommandAssaultPoint(followPoint);
                                }
                            
                            //selectedSosig.CommandAssaultPoint(followPoint);

                        }
                        /*if (Vector3.Distance(followPoint, selectedSosig.m_assaultPoint) > 6)
                        {
                            selectedSosig.CommandAssaultPoint(followPoint);
                        }
                             float oldSpeed = selectedSosig.Speed_Run;
                             while (Vector3.Distance(followPoint, selectedSosig.m_assaultPoint) > 25)
                             {
                                 selectedSosig.Speed_Run = 10;
                                 if (selectedSosig.Speed_Run != oldSpeed)
                                 {
                                     selectedSosig.Speed_Run = oldSpeed;
                                 }
                             }*/

                    }
                }
                //Debug.Log("updated the assault point");


                foreach (Sosig sosig in (spawnedChatters))
                {
                    if (sosig.BodyState == Sosig.SosigBodyState.Dead)
                    {
                        sosig.TickDownToClear(3);
                    }
                    if (sosig.Priority.HasFreshTarget() && sosig.CurrentOrder == Sosig.SosigOrder.Investigate && sosig.m_entityRecognition >= 0.65f)
                    {
                        sosig.SetCurrentOrder(Sosig.SosigOrder.Skirmish);
                    }
                }
            }
        }
        public ChatWatcher()
        {
            instance = this;
        }
        // public void addSosigToList(Sosig Jeff)
        // {
        //    spawnedChatters.Add(Jeff);
        //}
    }
}