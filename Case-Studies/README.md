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

This Powershell script packages user-provided files into an MSI installer file. When run, the script displas a GUI with user options. When the user clickes Start, 
the MSI installer will be built. From there, the user can run the MSI to install their files in the standard location (Program Files and Windows registry).
