﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Laba14_Tsykra
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TPole Pole;

        TRand Rand;

        public MainWindow()
        {
            InitializeComponent();

            Pole = new TPole(gPole);  //рисует поле и шашки

            Rand = new TRand(64);
        }

        private void cmClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        bool IsW = true;

        private void cmRun(object sender, RoutedEventArgs e)
        {
            if (IsW)
            {
                RunChecks(Pole.CW);
            }
            else
            {
                RunChecks(Pole.CB);      //вот тут ходит комп
            }

            IsW = !IsW;
        }

        void RunChecks(TChecks C)
        {
            TRuns Runs;
            TRuns RunsKill;

            Rand.Clear();

            for (int n = 0; n < C.Count; n++)
            {
                RunsKill = C[n].GetRunsKill(Pole);

                if (RunsKill.Count > 0)
                {
                    Rand.Add(n);
                }
            }

            if (Rand.Count > 0)
            {
                C[Rand.Get].Run(Pole, null);
                return;
            }

            Rand.Clear();

            for (int n = 0; n < C.Count; n++)
            {
                Runs = C[n].GetRuns(Pole);

                if (Runs.Count > 0)
                {
                    Rand.Add(n);
                }
            }

            if (Rand.Count > 0)
            {
                C[Rand.Get].Run(Pole, null);
            }
            else
            {
                IsGame = false;

                WB Cx;

                if (IsW)
                {
                    Cx = WB.W;
                }
                else
                {
                    Cx = WB.B;
                }

                Pole.GameOver(Cx);

                Pole = new TPole(gPole);
            }

        }

        bool IsGame;

        DispatcherTimer Timer;

        private void cmAutoRun(object sender, RoutedEventArgs e)
        {
            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(onTick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 10);

            IsGame = true;

            Timer.Start();
        }

        void onTick(object sender, EventArgs e)
        {
            if (IsGame)
            {
                cmRun(null, null);
            }
            else
            {
                Timer.Stop();
            }

        }

        private void cmPlay(object sender, RoutedEventArgs e)
        {

        }

        TCheck CheckFor = null;

        private void cmClick(object sender, MouseButtonEventArgs e)
        {
            Point XY = e.GetPosition(gPole);


            TCell Z = Pole.GetCell(XY);

            if (CheckFor == null)
            {
                Pole.Who(Z, out CheckFor);

                if (CheckFor == null)
                {
                    return;
                }

                if (CheckFor.C != WB.W)
                {
                    CheckFor = null;
                    return;
                }
            }
            else
            {
                ArrayList arr = new ArrayList();

                TRun Run = new TRun(CheckFor, Z);

                TRuns Runs;
                TRuns RunsKill;

                for (int n = 0; n < Pole.CW.Count; n++)
                {
                    RunsKill = Pole.CW[n].GetRunsKill(Pole);

                    for (int k = 0; k < RunsKill.Count; k++)
                    {
                        arr.Add(RunsKill[k]);
                    }
                }
                if (arr.Count > 0)
                {
                    for (int k = 0; k < arr.Count; k++)
                    {
                        TRun R = (TRun)arr[k];

                        if ((Run.Check.Pos.Eq(R.Check.Pos)) && (Run.PosTo.Eq(R.PosTo)))
                        {
                            R.Check.Run(Pole, R);

                            IsW = !IsW;
                            cmRun(null, null);
                            CheckFor = null;
                            return;
                        }
                    }

                    CheckFor = null;
                    return;
                }
                else
                {
                    arr.Clear();

                    for (int n = 0; n < Pole.CW.Count; n++)
                    {
                        Runs = Pole.CW[n].GetRuns(Pole);

                        for (int k = 0; k < Runs.Count; k++)
                        {
                            arr.Add(Runs[k]);
                        }
                    }

                    if (arr.Count > 0)
                    {
                        for (int k = 0; k < arr.Count; k++)
                        {
                            TRun R = (TRun)arr[k];

                            if ((Run.Check.Pos.Eq(R.Check.Pos)) && (Run.PosTo.Eq(R.PosTo)))
                            {
                                R.Check.Run(Pole, R);

                                IsW = !IsW;
                                cmRun(null, null);
                                CheckFor = null;
                                return;
                            }
                        }

                        CheckFor = null;
                        return;
                    }
                    else
                    {
                        IsGame = false;

                        WB Cx;

                        if (IsW)
                        {
                            Cx = WB.W;
                        }
                        else
                        {
                            Cx = WB.B;
                        }

                        Pole.GameOver(Cx);

                        Pole = new TPole(gPole);
                    }
                }
            }
        }
    }
}
