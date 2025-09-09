---
layout: post
title: "Operating Corona"
subtitle: "Simulating a mass spectrometer by replaying previously collected data"
date: 2025-09-05 12:00:00 -0700
background: '/img/Corona1.png'
---

#### Step 1: Queueing Data Files for MS Simulation

1. Select files using the ![Load Button]({{site.baseurl}}/img/posts/Operation/LoadButton.png) Load Button. Valid file formats are Thermo raw and mzML.

2. Set the desired run parameters. By default the data will be simulated in its entirety. It is possible to simulate acquisition of a subset of spectra
using either scan number or retention time boundaries.
![Run Parameters]({{site.baseurl}}/img/posts/Operation/RunParameters.png)

3. Select one or more rows from the file list to queue up and press the ![Add Button]({{site.baseurl}}/img/posts/Operation/AddButton.png) Add Run button.
The files now appear in the data file queue:

<p class="center">
<img src="{{site.baseurl}}/img/posts/Operation/FileQueue.png" />
</p>

<br/>
#### Step 2: Run the MS Simulation

1. Start the MS simulation by pressing the ![Play Button]({{site.baseurl}}/img/posts/Operation/PlayButton.png) Play Button. Data files will be used for
simulation in the order they are placed in the queue.

2. Pause the simulation at any time with the ![Pause Button]({{site.baseurl}}/img/posts/Operation/PauseButton.png) Pause Button. Alternatively, stop the
current data file simulation and move on to the next one in the queue with the ![Stop Button]({{site.baseurl}}/img/posts/Operation/StopButton.png) Stop Button.

3. Adjust the speed of the spectral playback by sliding the tab on the throttle. Playback can be increased to 100x speed.
<p class="center"><img src="{{site.baseurl}}/img/posts/Operation/Throttle.png" /></p>


<br/>
#### Step 3: Inspecting the MS Simulation

Switch to the Real Time Display to observe the MS simulation, inspect the chromatography, spectra, and any messages added to the Corona Message Log.

<p class="center">
<img src="{{site.baseurl}}/img/posts/Operation/RealTimeDisplay.png" />
</p>