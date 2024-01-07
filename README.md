# Programming Digital Twins - [Unity](https://unity.com/) Components
This is the source repository for [Unity](https://unity.com/)-based software and other application-specific components (written primarily in C#) related to my Digital Twins Programming course at Northeastern University. The intent of this repository is to provide students with a baseline [Unity](https://unity.com/) application / compute capability that can serve as the data ingestion and visualization functionality for the digital twin components (which are housed in a separate repository). For convenience to the reader, some of the basic functionality has already been implemented, with other key components requiring implementation by users of the repository (e.g., students taking my Digital Twins Programming course).

The code in this repository is largely comprised of classes that are designed to be partial-solutions only, with refinement provided by the reader / end user as appropriate. These classes and their relationships respresent a notional design that are configured to interact with the edge application components that are part of the [pdt-edge-components](https://github.com/programming-digital-twins/pdt-edge-components/) repository. These requirements encapsulate the programming exercises presented in my course [Building Digital Twins](TBD).

## Links, Exercises, Updates, Errata, and Clarifications

Please see the following links to access exercises for this project. Please note that many of the exercises and sample source code in this repository is based on many of the examples outlined in the [Programming the IoT Kanban Board](https://github.com/orgs/programming-the-iot/projects/1), which is aligned with my book, [Programming the Internet of Things Book](https://learning.oreilly.com/library/view/programming-the-internet/9781492081401/).
 - [Programming Digital Twins Requirements](https://github.com/orgs/programming-digital-twins/projects/1)
 - [Original Constrained Device Application Source Code Template](https://github.com/programming-the-iot/python-components)
 - [Programming Digital Twins Edge Components Source Code Template](https://github.com/programming-digital-twins/pdt-edge-components)
 - [Programming the Internet of Things Book](https://learning.oreilly.com/library/view/programming-the-internet/9781492081401/)

## How to use this repository
If you're reading [Programming the Internet of Things: An Introduction to Building Integrated, Device to Cloud IoT Solutions](https://learning.oreilly.com/library/view/programming-the-internet/9781492081401), you'll see a partial tie-in with the exercises described in each chapter and this repository.

## This repository aligns to exercises in Programming Digital Twins, and partially to Programming the Internet of Things
These components are all written in C# and are partially based on, although different from, the exercises designed for my book [Programming the Internet of Things: An Introduction to Building Integrated, Device to Cloud IoT Solutions](https://learning.oreilly.com/library/view/programming-the-internet/9781492081401).

## How to navigate the directory structure for this repository
This repository is comprised of the following key paths:
- [LabBenchStudios/ProgrammingDigitalTwins](https://github.com/programming-digital-twins/pdt-unity-components/tree/alpha/LabBenchStudios/ProgrammingDigitalTwins): All other assets are contained within this path.
  - [Materials](https://github.com/programming-digital-twins/pdt-unity-components/tree/alpha/LabBenchStudios/ProgrammingDigitalTwins/Materials): Contains some initial materials.
  - [Prefabs](https://github.com/programming-digital-twins/pdt-unity-components/tree/alpha/LabBenchStudios/ProgrammingDigitalTwins/Prefabs): Contains some initial prefabs.
  - [Scripts/Unity](https://github.com/programming-digital-twins/pdt-unity-components/tree/alpha/LabBenchStudios/ProgrammingDigitalTwins/Scripts/Unity): Contains all Unity-specific C# scripts.
    - [Common](https://github.com/programming-digital-twins/pdt-unity-components/tree/alpha/LabBenchStudios/ProgrammingDigitalTwins/Scripts/Unity/Common): Contains shared C# scripts.
    - [Controller](https://github.com/programming-digital-twins/pdt-unity-components/tree/alpha/LabBenchStudios/ProgrammingDigitalTwins/Scripts/Unity/Controller): Contains controller-specific C# scripts (e.g., rotation, etc.)
    - [Dashboard](https://github.com/programming-digital-twins/pdt-unity-components/tree/alpha/LabBenchStudios/ProgrammingDigitalTwins/Scripts/Unity/Dashboard): Contains dashboard-specific C# scripts (e.g., 2D / 3D test messaging).
    - [Manager](https://github.com/programming-digital-twins/pdt-unity-components/tree/alpha/LabBenchStudios/ProgrammingDigitalTwins/Scripts/Unity/Manager): Contains manager-related C# scripts (e.g., event handling).

Here are some other files at the top level that are important to review:
- [README.md](https://github.com/programming-digital-twins/pdt-unity-components/blob/alpha/README.md): This README.
- [LICENSE](https://github.com/programming-digital-twins/pdt-unity-components/blob/alpha/LICENSE): The repository's non-code artifact LICENSE file (e.g., documentation, prefabs, etc.)
- [LICENSE-CODE](https://github.com/programming-digital-twins/pdt-unity-components/blob/alpha/LICENSE-CODE): The repository's code artifact LICENSE file (e.g., source code [mostly C#])

NOTE: The directory structure and all files are subject to change based on feedback I receive from readers of my book and students in my IoT class, as well as improvements I find to be helpful for overall repo betterment.

# Other things to know

## Pull requests
PR's are disabled while the codebase is being developed.

## Updates
Much of this repository, and in particular unit and integration tests, will continue to evolve, so please check back regularly for potential updates. Please note that API changes can - and likely will - occur at any time.

# REFERENCES
This repository has external dependencies on other open source projects. I'm grateful to the open source community and authors / maintainers of the following libraries:

- [pdt-cfw-components](https://github.com/programming-digital-twins/pdt-cfw-components)
  - Reference: Andrew D. King. Programming Digital Twins Client Framework Components. (2024) [Online]. Available: https://github.com/programming-digital-twins/pdt-cfw-components.
- [DTDLParser](https://github.com/digitaltwinconsortium/DTDLParser)
  - Reference: Digital Twin Consortium and contributors. (2024) [Online]. Available: https://github.com/digitaltwinconsortium/DTDLParser.
- [InfluxDB.Client](https://github.com/influxdata/influxdb-client-csharp)
  - Reference: InfluxData, Inc. Influx DB C# Client. (2024) [Online]. Available: https://github.com/influxdata/influxdb-client-csharp.
- [MQTTnet](https://github.com/dotnet/MQTTnet)
  - Reference: .NET Foundation and contributors. MQTTnet .NET library for MQTT communications. (2024) [Online]. Available: https://github.com/dotnet/MQTTnet.
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
  - Reference: James Newton-King. Json.NET JSON framework for .NET. (2024) [Online]. Available: https://github.com/JamesNK/Newtonsoft.Json.
- [NUnit](https://nunit.org/)
  - Reference: Charlie Poole, Rob Prouse. Nunit unit testing framework for .NET languages. (2024) [Online]. Available: https://github.com/nunit.

NOTE: This list will be updated as others are incorporated.

# FAQ
For typical questions (and answers) to the repositories of the Programming the IoT project, please see the [FAQ](https://github.com/programming-the-iot/book-exercise-tasks/blob/default/FAQ.md).

# IMPORTANT NOTES
This code base is under active development.

If any code samples or other technology this work contains, describes, and / or is subject to open source licenses or the intellectual property rights of others, it is your responsibility to ensure that your use thereof complies with such licenses and/or rights.

# LICENSE
Please see [LICENSE](https://github.com/programming-digital-twins/pdt-unity-components/blob/alpha/LICENSE) if you plan to use these assets.
Please see [LICENSE-CODE](https://github.com/programming-digital-twins/pdt-unity-components/blob/alpha/LICENSE-CODE) if you plan to use this code.
