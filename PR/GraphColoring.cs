using Akka.Actor;
using Akka.Routing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PR
{
    class GraphColoring
    {
        private class slistEl
        {
            public slistEl next { get; set; }                // Następny element listy
            public int v { get; set; }                          // Wierzchołek docelowy
        };

        public void colorGraph()
        {
            int b, bc, n, m, i, u, v;
            int[] CT;
            slistEl[] graf;
            slistEl p, r;
            long cnt;         // Licznik prób
            bool test;

            n = Int32.Parse(Console.ReadLine());
            m = Int32.Parse(Console.ReadLine());

            graf = new slistEl[n];       // Tablica list sąsiedztwa
            for (i = 0; i < n; i++) graf[i] = null;

            CT = new int[n];               // Tablica kolorów wierzchołków

            // Odczytujemy krawędzie grafu

            for (i = 0; i < m; i++)
            {
                u = Int32.Parse(Console.ReadLine());
                v = Int32.Parse(Console.ReadLine());
                p = new slistEl();              // Tworzymy element listy
                p.v = u;
                p.next = graf[v];            // Element dołączamy do listy sąsiadów v
                graf[v] = p;

                p = new slistEl();              // To samo dla krawędzi w drugą stronę
                p.v = v;
                p.next = graf[u];            // Element dołączamy do listy sąsiadów u
                graf[u] = p;
            }

            // Rozpoczynamy algorytm kolorowania grafu

            cnt = 0;

            for (i = 0; i < n; i++) CT[i] = 0; // Inicjujemy licznik

            b = 2;                          // Zliczanie rozpoczynamy przy podstawie 2
            bc = 0;                         // Liczba najstarszych cyfr

            while (true)
            {
                if (bc != 0)                        // Kombinację sprawdzamy, gdy zawiera najstarszą cyfrę
                {
                    test = true;
                    cnt++;                      // Zwiększamy liczbę prób
                    for (v = 0; v < n; v++)      // Przeglądamy wierzchołki grafu
                    {
                        for (p = graf[v]; p!=null; p = p.next) // Przeglądamy sąsiadów wierzchołka v
                            if (CT[v] == CT[p.v])   // Testujemy pokolorowanie
                            {
                                test = false;         // Zaznaczamy porażkę
                                break;                // Opuszczamy pętlę for
                            }
                        if (!test) break;          // Opuszczamy pętlę for
                    }
                    if (test) break;             // Kombinacja znaleziona, kończymy pętlę główną
                }

                while (true)                   // Pętla modyfikacji licznika
                {
                    for (i = 0; i < n; i++)
                    {
                        CT[i]++;                 // Zwiększamy cyfrę
                        if (CT[i] == b - 1) bc++;
                        if (CT[i] < b) break;
                        CT[i] = 0;               // Zerujemy cyfrę
                        bc--;
                    }

                    if (i < n) break;           // Wychodzimy z pętli zwiększania licznika
                    b++;                       // Licznik się przewinął, zwiększamy bazę
                }
            }

            // Wyświetlamy wyniki

            Console.WriteLine();
            for (v = 0; v < n; v++)
                Console.WriteLine("vertex " + v + " has color " + CT[v]);
            Console.WriteLine();
            Console.WriteLine("graph chromatic number = " + b + "\n" + "number of tries = " + cnt + "\n");

            Console.ReadLine();
        }
    }
}
