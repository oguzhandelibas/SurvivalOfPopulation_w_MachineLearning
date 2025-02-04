﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MachineLearningBasic.SurvivalOfPopulation.Data
{
    public class PopulationManager : MonoBehaviour
    {

        public GameObject botPrefab;
        public int populationSize = 50;
        List<GameObject> population = new List<GameObject>();
        public static float elapsed = 5;
        public float trialTime = 5;
        int generation = 1;

        GUIStyle guiStyle = new GUIStyle();
        void OnGUI()
        {
            guiStyle.fontSize = 25;
            guiStyle.normal.textColor = Color.white;
            GUI.BeginGroup(new Rect(10, 10, 250, 150));
            GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
            GUI.Label(new Rect(10, 25, 200, 30), "Population Size: " + population.Count, guiStyle);
            GUI.Label(new Rect(10, 50, 200, 30), "Gen: " + generation, guiStyle);
            GUI.Label(new Rect(10, 75, 200, 30), string.Format("Time: {0:0.0}", elapsed), guiStyle);

            GUI.EndGroup();
        }


        // Use this for initialization
        void Start()
        {
            elapsed = trialTime;
            for (int i = 0; i < populationSize; i++)
            {
                Vector3 startingPos = new Vector3(this.transform.position.x + Random.Range(-5.0f, 5.0f),
                                                    this.transform.position.y,
                                                    this.transform.position.z + Random.Range(-5.0f, 5.0f));

                GameObject b = Instantiate(botPrefab, startingPos, this.transform.rotation);
                b.GetComponent<Brain>().Init();
                population.Add(b);
            }
        }

        GameObject Breed(GameObject parent1, GameObject parent2)
        {
            Vector3 startingPos = new Vector3(this.transform.position.x + Random.Range(-5.0f, 5.0f),
                                                    this.transform.position.y,
                                                    this.transform.position.z + Random.Range(-5.0f, 5.0f));
            GameObject offspring = Instantiate(botPrefab, startingPos, this.transform.rotation);
            Brain b = offspring.GetComponent<Brain>();
            if (Random.Range(0, 6) == 1) //mutate 1 in 15
            {
                b.Init();
                b.dna.Mutate();
            }
            else
            {
                b.Init();
                b.dna.Combine(parent1.GetComponent<Brain>().dna, parent2.GetComponent<Brain>().dna);
            }
            return offspring;
        }

        void BreedNewPopulation()
        {
            elapsed = trialTime;
            List<GameObject> sortedList = population.OrderBy(o => o.GetComponent<Brain>().timeAlive).ToList();

            population.Clear();
            for (int i = (int)(sortedList.Count / 2.0f) - 1; i < sortedList.Count - 1; i++)
            {
                population.Add(Breed(sortedList[i], sortedList[i + 1]));
                population.Add(Breed(sortedList[i + 1], sortedList[i]));
            }
            //destroy all parents and previous population
            for (int i = 0; i < sortedList.Count; i++)
            {
                Destroy(sortedList[i]);
            }
            generation++;
        }

        // Update is called once per frame
        void Update()
        {
            elapsed -= Time.deltaTime;
            if (elapsed <= 0)
            {
                elapsed = 0;
                BreedNewPopulation();
            }
        }
    }
}