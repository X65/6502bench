﻿/*
 * Copyright 2019 faddenSoft
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

using CommonWPF;

namespace MakeDist {
    /// <summary>
    /// Show progress log while copying files.
    /// </summary>
    public partial class CopyProgress : Window {
        /// <summary>
        /// Progress message, with colorful text.  This is generated by the worker thread and
        /// passed to the UI thread.
        /// </summary>
        public class ProgressMessage {
            public string Text { get; private set; }
            public Color Color { get; private set; }
            public bool HasColor { get { return Color.A != 0; } }

            public ProgressMessage(string msg) : this(msg, Colors.Transparent) { }

            public ProgressMessage(string msg, Color color) {
                Text = msg;
                Color = color;
            }
        }

        private BackgroundWorker mWorker;
        private FlowDocument mFlowDoc = new FlowDocument();
        private Color mDefaultColor;

        // Copy parameters.
        private FileCopier.BuildType mBuildType;
        private bool mCopyTestFiles;

        public CopyProgress(Window owner, FileCopier.BuildType buildType, bool copyTestFiles) {
            InitializeComponent();
            Owner = owner;
            DataContext = this;

            mBuildType = buildType;
            mCopyTestFiles = copyTestFiles;

            mDefaultColor = ((SolidColorBrush)progressRichTextBox.Foreground).Color;

            // Create and configure the BackgroundWorker.
            mWorker = new BackgroundWorker();
            mWorker.WorkerReportsProgress = true;
            mWorker.WorkerSupportsCancellation = true;
            mWorker.DoWork += BackgroundWorker_DoWork;
            mWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            mWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            progressRichTextBox.Document = mFlowDoc;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            mWorker.RunWorkerAsync();
        }

        // NOTE: executes on work thread.  DO NOT do any UI work here.  Pass the test
        // results through e.Result.
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
            BackgroundWorker worker = sender as BackgroundWorker;

            FileCopier copier = new FileCopier(mBuildType, mCopyTestFiles);
            e.Result = copier.CopyAllFiles(worker);

            if (worker.CancellationPending) {
                e.Cancel = true;
            }
        }

        // Callback that fires when a progress update is made.
        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            // We get progress from GenTest, and from the IAssembler/IGenerator classes.  This
            // is necessary to make cancellation work right, and allows us to show the
            // asm/gen progress messages if we want to.
            if (e.UserState is ProgressMessage) {
                ProgressMessage msg = e.UserState as ProgressMessage;
                if (msg.HasColor) {
                    progressRichTextBox.AppendText(msg.Text, msg.Color);
                } else {
                    // plain foreground text color
                    progressRichTextBox.AppendText(msg.Text, mDefaultColor);
                }
                progressRichTextBox.ScrollToEnd();
            } else {
                // Most progress updates have an e.ProgressPercentage value and a blank string.
                if (!string.IsNullOrEmpty((string)e.UserState)) {
                    Debug.WriteLine("Sub-progress: " + e.UserState);
                }
            }
        }

        // Callback that fires when execution completes.
        private void BackgroundWorker_RunWorkerCompleted(object sender,
                RunWorkerCompletedEventArgs e) {
            if (e.Cancelled) {
                Debug.WriteLine("Test halted -- user cancellation");
            } else if (e.Error != null) {
                Debug.WriteLine("Test failed: " + e.Error.ToString());
                progressRichTextBox.AppendText("\r\n");
                progressRichTextBox.AppendText(e.Error.ToString(), mDefaultColor);
                progressRichTextBox.ScrollToEnd();
            } else {
                Debug.WriteLine("Copy complete");
            }

            cancelButton.Content = "Close";
        }
    }
}
