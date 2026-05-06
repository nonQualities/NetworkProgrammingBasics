#include <iostream>
#include <vector>
using namespace std;

int main() {
    int n;
    cout << "Enter number of stations: ";
    cin >> n;

    vector<int> bitMap(n);

    cout << "\nEnter 1 (has data) or 0 (no data) for each station:\n";
    for (int i = 0; i < n; i++) {
        cout << "Station " << i << ": ";
        cin >> bitMap[i];
    }

    cout << "\nBit Map: ";
    for (int i = 0; i < n; i++) {
        cout << bitMap[i] << " ";
    }
    cout << endl;

    cout << "\nTransmission Order:\n";
    bool anyTransmission = false;

    for (int i = 0; i < n; i++) {
        if (bitMap[i] == 1) {
            cout << "Station " << i << " is transmitting...\n";
            anyTransmission = true;
        }
    }

    if (!anyTransmission) {
        cout << "No station has data to transmit.\n";
    }

    return 0;
}