---
layout: post
title: "Assessing Custom Scan Events"
subtitle: "Evaluating Outcomes of RTMS Applications"
date: 2025-09-05 10:30:00 -0700
background: '/img/Corona1.png'
---

The heart of any IAPI application is to trigger custom scan events external to the MS acquisition method in
response to observations in the incoming data.

The custom scan instructions are a text-based list of tuples: an instrument setting and its value. A custom
scan event can contain more than a dozen specific settings. Errors in the text-base list, typically
in the form of typos or invalid values, are ignored. In these cases, the instrument will either fail to
perform the custom scan, or perform a custom scan with suboptimal settings.

<p class="center">
<img width="600px" src="{{site.baseurl}}/img/posts/CustomScans/CustomScans.png" />
</p>

Corona contains a custom scan capture pane, logging all custom scan events received in a run. The exact
instrument settings and their values are listed for inspection. The easiest way to identify why a custom scan
event failed is to re-run the acquired data on Corona and inspect the custom scan even that preceded the failed
custom scan for errors.
