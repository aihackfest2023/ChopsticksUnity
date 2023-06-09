# ChopsticksUnity

Chopsticks RL attempt with Unity

# Folders and content

## ChopsticksUnity

Unity project folder with the Unity version of the chopsticks game, as well as the MLAgents training which is used to obtain an trained model in onnx format

There are some issues with the current controls in the demo version, to be fixed in the future.

## config

The config folder contains the configuration files for the training of the model. The config file is used by the MLAgents training to define the training parameters. The config file is also used by the MLAgents inference to define the inference parameters.

It contains a results folder. The `tensorboard --logdir results` command can be used to visualize the training results. The results folder also contains the trained model in onnx format.

## TestRunPython

This folder contains the colab notebook used to test the trained model. It also contains the python version of the trained model in onnx format.

## GameLogicPython

The GameLogicPython folder contains the python version of the chopsticks game. It is used to test the trained model. It also contains the python version of the trained model in onnx format.

The ChopsticksPythonAI.py has the implementation of a ChopsticksGame class which maintains the state of the game and implements the rules of the game. It uses the ChopstickPlayer.onnx model and calls it to make decisions for player 2

The ChopsticksPythonClassTest.py has the implementation of the code which is used to test the ChopsticksGame file. Examples of how to get the legal moves, send a move, and call player 2 can be found in the test file.

This was intended to be used for the backend, with a more intuitive ui to replace the frontend. However, due to time constraints, this is not available.

# How to run

## Unity

Open the Unity project folder with Unity. The project was created with Unity 2021.3.11f1.

MLAgents Version 2.0.1 was used for the training. The MLAgents package can be installed from the package manager.

## MLAgents training

The MLAgents training can be run with the following command:
`mlagents-learn ./new_trainer_config.yaml --run-id hb_03`

The following packages are required:

- `pip3 install torch~=1.7.1 -f https://download.pytorch.org/whl/torch_stable.html`
- `python -m pip install mlagents==0.28.0`
- `pip install importlib-metadata==4.4`

To upgrade the config file:
`python -m mlagents.trainers.upgrade_config trainer_config.yaml new_trainer_config.yaml`

## Python ChopsticksGame class

The following packages are required:

- `pip install onnxruntime`
- `pip install torch`
- `pip install tf2onnx`
- `pip install skl2onnx`

To run the test file:

- `conda activate onnx-reader-test-aihackfest`
- `python3 ChopsticksPythonClassTest.py`

# Training
Individual Game set up

![GameScene](https://github.com/aihackfest2023/ChopsticksUnity/assets/53657436/c494ba45-527d-44a6-8da0-1869872c55dc)

Training multiple Agents simultaneously

![Model training arenas](https://github.com/aihackfest2023/ChopsticksUnity/assets/53657436/78c77166-5cfe-492e-b456-9e0c150f92fe)

Results of training with Tensorboard

![Model training result](https://github.com/aihackfest2023/ChopsticksUnity/assets/53657436/770cce04-a97f-4610-bc91-ab8a23b04041)
