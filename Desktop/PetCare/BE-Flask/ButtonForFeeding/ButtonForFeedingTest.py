import unittest as test
import ButtonForFeedingModel
import time

class ButtonFeedingTest(test.TestCase):
    """
        Clasa ce testeaza clasa ButtonFeeding;
        Pentru fiecare clasa vom face diferite teste in functie de ce functii are
    """

    def setUp(self):
        # Aici vom face o instanta a clasei noastre pe care vrem sa o testam
        self.feedingButton = ButtonForFeedingModel.ButtonForFeeding(10,1,'')

    def test_sensor(self):
        """
            Aceasta functie ar trebui sa verifice daca senzorul functioneaza optinem 30 la feeding
        """
        self.feedingButton.makeFeedingEmpty()
        self.feedingButton.startSensor()
        time.sleep(4)
        self.feedingButton.stopSensor()
        self.assertEqual(self.feedingButton.getFeedingLevel(), 30, 'Feeding level should be 30.')
    

if __name__ == '__main__':
    test.main()