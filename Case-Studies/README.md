# Case Studies

This folder contains tools that I've built that have solved problems in my environment.

## Microsoft Office Installer

This Powershell script installs Microsoft Office. When run, the script displays a GUI with user options. When the user clicks Start, the installer will install Office.

### Overview

This project is a PowerShell-based GUI application that simplifies the deployment of Microsoft Office using the Office Deployment Tool (ODT). It abstracts a complex, multi-step deployment process into a simple user interface, allowing non-technical users to install Office with the correct configuration, adapting to compliance restrictions, and supporting seamless software updates.

### Problem

Deploying Microsoft Office in an enterprise environment introduced several challenges:
- Installations needed to work in offline or restricted network environments
- Users need control over which applications to install
- Installations had to comply with organizational policies, limiting use of some applications
- Updates needed to be centrally managed without relying on internet access
- The native ODT process required manual XML configuration, which is not user-friendly and error-prone
- C# is to be avoided due to additional compliance overhead (due to being a compiled language)

### Solution
I built a PowerShell GUI wrapper around the Office Deployment Tool that:
- Collects user input through a simple interface
- Dynamically generates a valid ODT configuration XML file
- Executes the ODT installation using that configuration file
- Automatically routes installations to the correct internal file share (which there are several)

### How It Works
1. User Interaction Layer (GUI)
   - A minimal interface is presented
   - User selects architecture (32-bit or 64-bit)
   - User selects applications (Word, Excel, Access)
   - Start the installation process
3. Configuration Generation
   - Based on user input, the script dynamically builds an XML file
   - Architecture and application selections are added to the XML
   - File share paths are selected appropriately
   - Software is configured to automatically update when software is updated on the file shares
4. Installation Execution
   - Once the XML is generated, the script invokes ODT
   - This delegates the actual installation to Microsoft’s supported tooling while maintaining full control over configuration

### Impact
- Reduced installation errors caused by manual configuration
- Enabled consistent Office deployments across multiple environments
- Simplified onboarding and support processes
- Improved update management through centralized control
- Reduced the need for additional instructional documentation (although the process is documented as well)

## MSI Baler

### Overview
MSI Baler is a PowerShell-based GUI tool that converts arbitrary application files (typically portable executables) into standardized MSI installers using the WiX Toolset. It was designed to bridge the gap between unmanaged portable software and enterprise compliance requirements, enabling consistent tracking, deployment, and lifecycle management.

### Problem
In enterprise environments, software inventory is often tracked via:
- Windows Registry entries
- Installed application metadata (MSI-based installs) stored in Windows registry

However, portable applications introduce several issues:
- No registry footprint means software is not discoverable by automated tools
- Manual tracking is error-prone and inconsistent
- No standardized installation process
- Compliance gaps in software reporting

This creates ongoing friction between:
- System owners (who need flexibility)
- Compliance teams (who need visibility and control)

### Solution
MSI Baler packages arbitrary file sets into valid MSI installers, allowing portable applications to behave like fully installed software.

The tool:
- Wraps files into an MSI package
- Registers the application in the Windows registry
- Enables automated discovery by existing infrastructure tools

### How It Works
1. User Interaction Layer (GUI)
   - A minimal interface is presented
   - User specifies input and output directories
   - User specifies application name and version (to store in Windows registry)
   - User specifies CPU architecture (auto-detected or manually specified)
   - The UI is structured to ensure only valid, compliant installers are created.
2. Configuration Generation
   - Based on user input, the script dynamically builds an XML definition file (.wxs)
   - Architecture and application selections are added to the XML
   - Input files are mapped to WiX components in a tree structure
   - Behavior of the MSI installer is standardized
3. WiX Build Pipeline
   - The .wxs file is compiled to a .wixobj file (using candle.exe)
   - The .wixobj file is linked to create the final .msi file (using light.exe)
4. MSI Output
   - The resulting MSI nstalls files into Program Files
   - Writes application metadata to the Windows registry
   - Applications are now discoverable by enterprise inventory systems

### Impact
- Eliminated manual tracking of portable applications
- Enabled full visibility in enterprise software inventory systems
- Reduced compliance gaps and audit overhead
- Standardized how software is packaged and deployed internally
