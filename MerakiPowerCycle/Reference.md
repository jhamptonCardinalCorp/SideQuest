# This script, for a given API key, and org ID
# For each network, get the SM devices and print to screen
# Mandatory arguments:
# -k <API KEY>      : Your Meraki Dashboard API Key
# -o <Org. ID>      : Your Meraki Org ID
# Pre requisites:
# Meraki library : pip install meraki : https://developer.cisco.com/meraki/api/#/python/getting-started

import meraki
import sys, getopt


def main(argv):

    print("Meraki Library version: ")
    print(meraki.__version__)

    arg_apikey = False

    try:
        opts, args = getopt.getopt(argv, 'k:o:')
    except getopt.GetOptError:
        printhelp(argv)
        sys.exit(2)

    for opt, arg in opts:
        if opt == '-k':
            arg_apikey = arg
        if opt == "-o":
            arg_orgId = arg

    # Create Meraki Client Object and initialise
    client = meraki.DashboardAPI(api_key=arg_apikey)

    networks = client.organizations.getOrganizationNetworks(organizationId=arg_orgId)

    print("networkID, model, osName, systemModel")
    for network in networks:
        networkName = (network['name'])
        networkID = (network['id'])

        try:
            devices = client.sm.getNetworkSmDevices(networkId=networkID, fields={"ip"})
            if len(devices) == 0:
                # no devices
                print("No devices in this SM network")
            else:
                for device in devices:
                    print(networkID, ",", device["name"], ",", device["osName"], ",", device["phoneNumber"])

        except meraki.APIError as e:
            print("No SM in " + networkName + " network")


def printhelp():
    # prints help information
    print('This is a script to Get the SM devices and their phone number across multiple networks.')
    print('')
    print('Mandatory arguments:')
    print(' -k <api key>         : Your Meraki Dashboard API key')
    print(' -o <Org. ID>         : Your Meraki Org ID')


if __name__ == '__main__':
    main(sys.argv[1:])