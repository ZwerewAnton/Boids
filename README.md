# Boids

![Unity](https://img.shields.io/badge/Unity-6000.3_LTS-black?logo=unity)

A Unity learning project focused on implementing and comparing different approaches to the classic **Boids (flocking) algorithm**.

The project explores both a traditional **MonoBehaviour-based approach with Jobs/Burst** and a **DOTS-based solution (Entities, Jobs, Burst)**, with an emphasis on performance, architecture, and scalability.

![Preview](https://raw.githubusercontent.com/ZwerewAnton/Media/refs/heads/master/Boids/preview.gif)

---

## About the Project

**Boids** is a personal Unity project created for learning and experimentation purposes.

The main goal of the project is to:
- implement the Boids algorithm in multiple ways,
- compare architectural approaches,
- explore Unity performance tools such as **Jobs**, **Burst**, and **DOTS**.

Two independent implementations are provided and can be run side by side for comparison.

---

## Implemented Approaches

### 1️⃣ MonoBehaviour + Jobs + Burst

- Classic GameObject / MonoBehaviour architecture
- Multithreaded logic using **Unity Jobs**
- Performance-critical code optimized with **Burst**
- Uses ScriptableObjects for configuration
- Easier to understand and closer to traditional Unity workflows

📂 Scene: `Assets/Scenes/Burst/BoidsBurstScene.unity`  
📂 Configs: `Assets/ScriptableObjects/Configs/BoidsBurstConfigs`


### 2️⃣ DOTS (Entities + Jobs + Burst)

- Fully data-oriented implementation using **Unity DOTS**
- Boids logic implemented with **Entities**, **Jobs**, and **Burst**
- Custom **Spatial Hash** used to optimize neighbor lookup
- Designed to scale to a much larger number of agents
- Demonstrates a more complex but highly performant architecture

📂 Scene: `Assets/Scenes/DOTS/BoidsDOTSScene.unity`  
📂 Configs: `Assets/ScriptableObjects/Configs/BoidsDOTSConfigs`

---

## Getting Started

1. Clone the repository
2. Open the project via **Unity Hub**
3. Use **Unity 6000.x LTS**
4. Choose one of the following scenes depending on the implementation you want to explore:
   - **MonoBehaviour + Jobs + Burst:**  
     `Assets/Scenes/Burst/BoidsBurstScene.unity`
   - **DOTS implementation:**  
     `Assets/Scenes/DOTS/BoidsDOTSScene.unity`

---

## Tech Stack

- **Unity:** 6000.x LTS
- **Multithreading:** Unity Jobs System
- **Performance:** Burst Compiler
- **DOTS:** Entities, Jobs, Burst
- **Rendering:** Universal Render Pipeline (URP)
