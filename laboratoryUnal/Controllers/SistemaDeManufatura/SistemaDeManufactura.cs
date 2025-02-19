﻿using EngineIO;
using System;
using System.Threading;

namespace Controllers.Scenes.SistemaDeManufactura
{
    public class SistemaDeManufactura : Controller
    {
        //Automatic or manual
        readonly MemoryBit manual;
        readonly MemoryBit automatic;

        //READER
        string newState;
        string newStateName;
        bool changeStateMessagePrinted;

        //SUPERVISOR
        bool supervisoryApproval;
        readonly SistemaDeManufacturaSupervisor sistemaDeManufaturaSupervisor;
        int e1Counter;

        //ESTEIRAS E1 E2
        //E1
        readonly MemoryBit conveyorE1;
        readonly MemoryBit conveyorE1Start;
        readonly MemoryBit conveyorE1Stop;
        readonly MemoryBit conveyorE1StartLight;
        readonly MemoryBit conveyorE1StopLight;
        readonly MemoryBit sensorEndE1;
        readonly MemoryInt sensorColor;
        readonly MemoryBit stopbladeEndE1;
        private enum E1ConveyorState
        {
            EMITTING,
            EMITTED,
            GOING_TO_ROBO0,
            REACHED_ROBO0,
            E2_TO_E1,
        }
        E1ConveyorState e1ConveyorState;
        private enum E2toE1Steps
        {
            GOING_TO_E2,
            DOWN_LOOKING_FOR_PIECE,
            GRABBING_PIECE,
            UP_WITH_PIECE,
            GOING_TO_E1_FIRST_HALF,
            GOING_TO_E1_SECOND_HALF,
            DOWN_WITH_PIECE,
            UNGRABBING_PIECE,
            UP_WITHOUT_PIECE,
            UNROTATE_FIRST_HALF,
            UNROTATE_SECOND_HALF
        }
        E2toE1Steps e2toE1Steps;

        //Emitter Remover
        readonly MemoryBit remover;
        readonly MemoryBit sensorEmitter;
        
        readonly MemoryInt emitterPart;
        readonly MemoryBit emitter;
        readonly MemoryInt emitterBase;
        Random random;
        int randomNumber;

        //E2
        readonly MemoryBit sensorStartE2;
        readonly RTRIG rtSensorStartE2;
        readonly MemoryFloat conveyorStartE2;
        readonly MemoryFloat conveyorPostStartE2;
        readonly MemoryFloat conveyorFirstCornerE2;
        readonly MemoryFloat conveyorMiddleE2;
        readonly MemoryFloat conveyorSecondCornerE2;
        readonly MemoryFloat conveyorPreEndE2;
        readonly MemoryFloat conveyorPrePreEndE2;
        readonly MemoryFloat conveyorEndE2;
        readonly MemoryBit sensorPreEndE2;
        readonly FTRIG ftSensorPreEndE2;
        readonly RTRIG rtSensorPreEndE2;
        readonly MemoryBit sensorSecondSpotE2;
        readonly MemoryBit sensorThirdSpotE2;
        readonly MemoryBit sensorFourthSpotE2;
        readonly MemoryBit sensorFifthSpotE2;
        readonly MemoryBit sensorSixthSpotE2;
        readonly MemoryBit sensorSeventhSpotE2;
        readonly MemoryBit sensorEighthSpotE2;
        readonly MemoryBit sensorNinthSpotE2;
        readonly MemoryBit sensorTenthSpotE2;
        readonly MemoryBit sensorAfterBlade;
        readonly MemoryBit sensorAtRobotArm;
        readonly MemoryBit sensorBeforeBuffer;
        readonly RTRIG rtBeforeBuffer;
        readonly FTRIG ftBeforeBuffer;
        readonly FTRIG ftAfterBlade;
        readonly RTRIG rtAfterBlade;
        readonly FTRIG ftAtRobotArm;
        readonly MemoryBit E2Stopblade;
        int e2PieceCounter;
        private enum BufferE2
        {
            ZERO,
            ONE,
            TWO,
            THREE,
            FOUR,
            FIVE,
            SIX,
            SEVEN,
            EIGHT,
            NINE,
            TEN,
            ELEVEN,
            TWELVE
        }
        BufferE2 bufferE2;
        private enum BufferE2LoadingStage
        {
            IDLE,
            START_LOADING,
            SEPARATE_PIECES,
            RESTORING_BUFFER_ORDER,
            REACHING_ROBOT_ARM
        }
        BufferE2LoadingStage bufferE2LoadingStage;

        private enum E2BeforeBuffer
        {
            IDLE,
            TURN_OFF_CONVEYORS,
            TAKING_PIECES_TO_BUFFER,
            WAITING
        }
        E2BeforeBuffer e2BeforeBuffer;
        private enum E2ConveyorsOnOff
        {
            CONVEYORS_OFF,
            CONVEYORS_ON
        }
        E2ConveyorsOnOff e2ConveyorsOnOff;
        //GripperArm
        readonly MemoryFloat armX;
        readonly MemoryFloat armXpos;
        readonly MemoryFloat armZ;
        readonly MemoryFloat armZpos;
        readonly MemoryBit armGrab;
        readonly MemoryBit armRotate;
        readonly MemoryBit armUnrotate;
        readonly MemoryBit armPieceRotate;
        readonly MemoryBit armRotating;
        readonly MemoryBit armPieceDetected;
        bool rotationBool;

        //ROBÔ
        readonly SistemaDeManufactura_Robo0 robo0;
        readonly MemoryBit startC1toB1;
        readonly MemoryBit startC2toB1;
        readonly MemoryBit startC2toB2;
        readonly MemoryBit startC3toB3;
        readonly MemoryBit startC1toE2;
        readonly MemoryBit startC2toE2;
        readonly MemoryBit startC3toE2;
        readonly MemoryBit stopbladeB1;
        readonly MemoryBit stopbladeB2;
        readonly MemoryBit stopbladeB3;
        readonly MemoryBit roboC1toB1Lights;
        readonly MemoryBit roboC2toB1Lights;
        readonly MemoryBit roboC2toB2Lights;
        readonly MemoryBit roboC3toB3Lights;
        readonly MemoryBit roboC1toE2Lights;
        readonly MemoryBit roboC2toE2Lights;
        readonly MemoryBit roboC3toE2Lights;
        int roboCounter;
        string color;
        bool roboFinished;
        private enum Robo0State
        {
            IDLE,
            E1toE2,
            E1toB1,
            E1toB2,
            E1toB3
        }
        Robo0State robo0State;

        //M1
        readonly MemoryBit startC2fromB2toM1;
        readonly MemoryBit startC2fromB1toM1;
        readonly MemoryBit startC1fromB1toM1;
        readonly MemoryBit startC3fromB3toM1;
        readonly MemoryBit sensorM1end;
        readonly MemoryBit m1C1fromB1Lights;
        readonly MemoryBit m1C2fromB1Lights;
        readonly MemoryBit m1C2fromB2Lights;
        readonly MemoryBit m1C3fromB3Lights;
        int m1Counter;
        bool roboM1Finished;
        bool bool_M1_c3;
        bool bool_M1_c2;
        bool bool_M1_c1;
        bool bool_M1_c2_b1;
        bool m1message1Printed;
        bool m1message2Printed;
        FTRIG ftAtM1end;

        private enum M1states
        {
            IDLE,
            C1fromB1toM1,
            C2fromB1toM1,
            C2fromB2toM1,
            C3fromB3toM1
        }
        M1states m1states;
        readonly SistemaDeManufatura_M1 roboM1;

        //Messages only once
        bool initialMessage;
        bool colorMessage;
        bool colorMessage2;
        public SistemaDeManufactura()
        {
            //automatic or manual
            manual = MemoryMap.Instance.GetBit("0: Manual", MemoryType.Input);
            automatic = MemoryMap.Instance.GetBit("1: Automatic", MemoryType.Input);

            //READER
            newState = "";
            newStateName = "";
            changeStateMessagePrinted = false;

            //SUPERVISOR
            supervisoryApproval = false;
            sistemaDeManufaturaSupervisor = new SistemaDeManufacturaSupervisor();
            e1Counter = 0;

            //ESTEIRAS E1 E2
            //E1
            conveyorE1 = MemoryMap.Instance.GetBit("Belt Conveyor (4m) 2", MemoryType.Output);
            conveyorE1Start = MemoryMap.Instance.GetBit("Liga esteira E1", MemoryType.Input);
            conveyorE1Stop = MemoryMap.Instance.GetBit("Desliga esteira E1", MemoryType.Input);
            conveyorE1StartLight = MemoryMap.Instance.GetBit("Start E1 (Light)", MemoryType.Output);
            conveyorE1StopLight = MemoryMap.Instance.GetBit("Stop E1 (Light)", MemoryType.Output);
            roboC1toB1Lights = MemoryMap.Instance.GetBit("Rôbo C1 to B1 (Light)", MemoryType.Output);
            roboC2toB1Lights = MemoryMap.Instance.GetBit("Rôbo C2 to B1 (Light)", MemoryType.Output);
            roboC2toB2Lights = MemoryMap.Instance.GetBit("Rôbo C2 to B2 (Light)", MemoryType.Output);
            roboC3toB3Lights = MemoryMap.Instance.GetBit("Rôbo C3 to B3 (Light)", MemoryType.Output);
            roboC1toE2Lights = MemoryMap.Instance.GetBit("Rôbo C1 to E2 (Light)", MemoryType.Output);
            roboC2toE2Lights = MemoryMap.Instance.GetBit("Rôbo C2 to E2 (Light)", MemoryType.Output);
            roboC3toE2Lights = MemoryMap.Instance.GetBit("Rôbo C3 to E2 (Light)", MemoryType.Output);
            sensorEndE1 = MemoryMap.Instance.GetBit("Diffuse Sensor 0", MemoryType.Input);
            sensorColor = MemoryMap.Instance.GetInt("Vision Sensor 0 (Value)", MemoryType.Input);
            stopbladeEndE1 = MemoryMap.Instance.GetBit("Stop Blade 1", MemoryType.Output);
            stopbladeEndE1.Value = true;
            e1ConveyorState = E1ConveyorState.EMITTING;
            e2toE1Steps = E2toE1Steps.GOING_TO_E2;

            //Emitter Remover
            remover = MemoryMap.Instance.GetBit("Remover 0 (Remove)", MemoryType.Output);
            remover.Value = true;
            sensorEmitter = MemoryMap.Instance.GetBit("Diffuse Sensor 11", MemoryType.Input);

            emitter = MemoryMap.Instance.GetBit("Emitter 2 (Emit)", MemoryType.Output);
            emitter.Value = false;
            emitterPart = MemoryMap.Instance.GetInt("Emitter 2 (Part)", MemoryType.Output);
            emitterPart.Value = 512;//512 is metal base, 1024 is blue lid, 2048 is green lid
            emitterBase = MemoryMap.Instance.GetInt("Emitter 2 (Base)", MemoryType.Output);
            emitterBase.Value = 0;
            random = new Random();
            randomNumber = 0;

            //E2
            sensorStartE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 1", MemoryType.Input);
            rtSensorStartE2 = new RTRIG();
            conveyorStartE2 = MemoryMap.Instance.GetFloat("Belt Conveyor (6m) 0 (V)", MemoryType.Output);
            conveyorStartE2.Value = 0;
            conveyorPostStartE2 = MemoryMap.Instance.GetFloat("Belt Conveyor (4m) 5 (V)", MemoryType.Output);
            conveyorFirstCornerE2 = MemoryMap.Instance.GetFloat("Curved Belt Conveyor 0 (V)", MemoryType.Output);
            conveyorMiddleE2 = MemoryMap.Instance.GetFloat("Belt Conveyor (4m) 0 (V)", MemoryType.Output);
            conveyorSecondCornerE2 = MemoryMap.Instance.GetFloat("Curved Belt Conveyor 1 (V)", MemoryType.Output);
            conveyorPrePreEndE2 = MemoryMap.Instance.GetFloat("Belt Conveyor (4m) 3 (V)", MemoryType.Output);
            conveyorPreEndE2 = MemoryMap.Instance.GetFloat("Belt Conveyor (4m) 4 (V)", MemoryType.Output);
            conveyorEndE2 = MemoryMap.Instance.GetFloat("Belt Conveyor (4m) 1 (V)", MemoryType.Output);
            bufferE2 = BufferE2.ZERO;
            sensorPreEndE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 2", MemoryType.Input);
            ftSensorPreEndE2 = new FTRIG();
            rtSensorPreEndE2 = new RTRIG();
            sensorSecondSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 12", MemoryType.Input);
            sensorThirdSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 13", MemoryType.Input);
            sensorFourthSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 14", MemoryType.Input);
            sensorFifthSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 3", MemoryType.Input);
            sensorSixthSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 16", MemoryType.Input);
            sensorSeventhSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 15", MemoryType.Input);
            sensorEighthSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 17", MemoryType.Input);
            sensorNinthSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 19", MemoryType.Input);
            sensorTenthSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 21", MemoryType.Input);
            sensorAfterBlade = MemoryMap.Instance.GetBit("Diffuse Sensor 23", MemoryType.Input);
            sensorAtRobotArm = MemoryMap.Instance.GetBit("Diffuse Sensor 24", MemoryType.Input);
            sensorBeforeBuffer = MemoryMap.Instance.GetBit("Diffuse Sensor 25", MemoryType.Input);
            rtBeforeBuffer = new RTRIG();
            ftBeforeBuffer = new FTRIG();
            ftAfterBlade = new FTRIG();
            rtAfterBlade = new RTRIG();
            ftAtRobotArm = new FTRIG();
            E2Stopblade = MemoryMap.Instance.GetBit("Stop Blade 5", MemoryType.Output);
            bufferE2LoadingStage = BufferE2LoadingStage.IDLE;
            e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_OFF;
            e2BeforeBuffer = E2BeforeBuffer.IDLE;
            e2PieceCounter = 0;

            //GripperArm
            armX = MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 X Set Point (V)", MemoryType.Output);
            armX.Value = 2.3f;
            armXpos = MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 X Position (V)", MemoryType.Input);
            armZ = MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 Z Set Point (V)", MemoryType.Output);
            armZ.Value = 6.0f;
            armZpos = MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 Z Position (V)", MemoryType.Input);
            armGrab = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 (Grab)", MemoryType.Output);
            armRotate = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 Rotate CCW", MemoryType.Output);
            armUnrotate = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 Rotate CW", MemoryType.Output);
            armPieceRotate = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 Gripper CCW", MemoryType.Output);
            armRotating = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 (Rotating)", MemoryType.Input);
            armPieceDetected = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 (Item Detected)", MemoryType.Input);
            rotationBool = false;


            //ROBÔ0
            startC1toB1 = MemoryMap.Instance.GetBit("Robô C1 to B1", MemoryType.Input);
            startC2toB1 = MemoryMap.Instance.GetBit("Robô C2 to B1", MemoryType.Input);
            startC2toB2 = MemoryMap.Instance.GetBit("Robô C2 to B2", MemoryType.Input);
            startC3toB3 = MemoryMap.Instance.GetBit("Robô C3 to B3", MemoryType.Input);
            startC1toE2 = MemoryMap.Instance.GetBit("Robô C1 to E2", MemoryType.Input);
            startC2toE2 = MemoryMap.Instance.GetBit("Robô C2 to E2", MemoryType.Input);
            startC3toE2 = MemoryMap.Instance.GetBit("Robô C3 to E2", MemoryType.Input);
            stopbladeB1 = MemoryMap.Instance.GetBit("Stop Blade 4", MemoryType.Output);
            stopbladeB1.Value = true;
            stopbladeB2 = MemoryMap.Instance.GetBit("Stop Blade 3", MemoryType.Output);
            stopbladeB2.Value = true;
            stopbladeB3 = MemoryMap.Instance.GetBit("Stop Blade 2", MemoryType.Output);
            stopbladeB3.Value = true;
            robo0State = Robo0State.IDLE;
            roboCounter = 0;
            color = "";
            roboFinished = false;

            robo0 = new SistemaDeManufactura_Robo0(
                MemoryMap.Instance.GetFloat("Pick & Place 0 X Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetFloat("Pick & Place 0 X Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Pick & Place 0 Y Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetFloat("Pick & Place 0 Y Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Pick & Place 0 Z Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetFloat("Pick & Place 0 Z Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetBit("Pick & Place 0 (Grab)", MemoryType.Output),
                MemoryMap.Instance.GetBit("Pick & Place 0 (Box Detected)", MemoryType.Input),
                MemoryMap.Instance.GetBit("Pick & Place 0 C(+)", MemoryType.Output),
                stopbladeEndE1, sistemaDeManufaturaSupervisor);

            //M1
            startC2fromB2toM1 = MemoryMap.Instance.GetBit("C2fromB2toM1", MemoryType.Input);
            startC2fromB1toM1 = MemoryMap.Instance.GetBit("C2fromB1toM1", MemoryType.Input);
            startC1fromB1toM1 = MemoryMap.Instance.GetBit("C1fromB1toM1", MemoryType.Input);
            startC3fromB3toM1 = MemoryMap.Instance.GetBit("C3fromB3toM1", MemoryType.Input);
            m1C1fromB1Lights = MemoryMap.Instance.GetBit("m1 C1 from B1 (Light)", MemoryType.Output);
            m1C2fromB1Lights = MemoryMap.Instance.GetBit("m1 C2 from B1 (Light)", MemoryType.Output);
            m1C2fromB2Lights = MemoryMap.Instance.GetBit("m1 C2 from B2 (Light)", MemoryType.Output);
            m1C3fromB3Lights = MemoryMap.Instance.GetBit("m1 C3 from B3 (Light)", MemoryType.Output);
            m1states = M1states.IDLE;
            sensorM1end = MemoryMap.Instance.GetBit("Diffuse Sensor 22", MemoryType.Input);
            roboM1 = new SistemaDeManufatura_M1(
                MemoryMap.Instance.GetFloat("Pick & Place 1 X Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetFloat("Pick & Place 1 X Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Pick & Place 1 Y Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetFloat("Pick & Place 1 Y Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Pick & Place 1 Z Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetFloat("Pick & Place 1 Z Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetBit("Pick & Place 1 (Grab)", MemoryType.Output),
                MemoryMap.Instance.GetBit("Pick & Place 1 (Box Detected)", MemoryType.Input),
                MemoryMap.Instance.GetBit("Belt Conveyor (2m) 5", MemoryType.Output),
                MemoryMap.Instance.GetBit("Belt Conveyor (2m) 4", MemoryType.Output),
                MemoryMap.Instance.GetBit("Belt Conveyor (2m) 3", MemoryType.Output),
                MemoryMap.Instance.GetBit("Belt Conveyor (2m) 0", MemoryType.Output),
                MemoryMap.Instance.GetBit("Diffuse Sensor 4", MemoryType.Input),
                MemoryMap.Instance.GetBit("Diffuse Sensor 5", MemoryType.Input),
                MemoryMap.Instance.GetBit("Diffuse Sensor 6", MemoryType.Input),
                MemoryMap.Instance.GetBit("Diffuse Sensor 9", MemoryType.Input),
                MemoryMap.Instance.GetBit("Diffuse Sensor 8", MemoryType.Input),
                MemoryMap.Instance.GetBit("Diffuse Sensor 7", MemoryType.Input),
                sensorM1end
                );
            m1Counter = 0;
            roboM1Finished = false;
            bool_M1_c3 = false;
            bool_M1_c2 = false;
            bool_M1_c1 = false;
            bool_M1_c2_b1 = false;
            m1message1Printed = false;
            m1message2Printed = false;
            ftAtM1end = new FTRIG();


            //Messages only once
            initialMessage = true;
            colorMessage = true;
            colorMessage2 = false;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            if (initialMessage)
            {
                Console.WriteLine("\n");
                Console.WriteLine("Green is C1, Metal is C2, and Blue is C3");
                Console.WriteLine("Product A is C2 and C1. Product B is C2 and C3.");
                Console.WriteLine("\n");
                sistemaDeManufaturaSupervisor.CreateController();
                initialMessage = false;
            }

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BUTTON LIGHTS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            {
                //E1
                if (sistemaDeManufaturaSupervisor.IsInActiveEventsLights("E1_on"))
                    conveyorE1StartLight.Value = true;
                else
                    conveyorE1StartLight.Value = false;

                if (sistemaDeManufaturaSupervisor.IsInActiveEventsLights("E1_off"))
                    conveyorE1StopLight.Value = true;
                else
                    conveyorE1StopLight.Value = false;

                //Rôbo
                if (sistemaDeManufaturaSupervisor.IsInActiveEventsLights("R_c1_b1"))
                    roboC1toB1Lights.Value = true;
                else
                    roboC1toB1Lights.Value = false;

                if (sistemaDeManufaturaSupervisor.IsInActiveEventsLights("R_c2_b1"))
                    roboC2toB1Lights.Value = true;
                else
                    roboC2toB1Lights.Value = false;

                if (sistemaDeManufaturaSupervisor.IsInActiveEventsLights("R_c2_b2"))
                    roboC2toB2Lights.Value = true;
                else
                    roboC2toB2Lights.Value = false;

                if (sistemaDeManufaturaSupervisor.IsInActiveEventsLights("R_c3_b3"))
                    roboC3toB3Lights.Value = true;
                else
                    roboC3toB3Lights.Value = false;

                if (sistemaDeManufaturaSupervisor.IsInActiveEventsLights("R_c1_e2"))
                    roboC1toE2Lights.Value = true;
                else
                    roboC1toE2Lights.Value = false;

                if (sistemaDeManufaturaSupervisor.IsInActiveEventsLights("R_c2_e2"))
                    roboC2toE2Lights.Value = true;
                else
                    roboC2toE2Lights.Value = false;

                if (sistemaDeManufaturaSupervisor.IsInActiveEventsLights("R_c3_e2"))
                    roboC3toE2Lights.Value = true;
                else
                    roboC3toE2Lights.Value = false;

                if (sistemaDeManufaturaSupervisor.IsInActiveEventsLights("M1_c2") && !m1message1Printed && !m1message2Printed)
                    m1C2fromB2Lights.Value = true;
                else
                    m1C2fromB2Lights.Value = false;

                if (sistemaDeManufaturaSupervisor.IsInActiveEventsLights("M1_c2_b1") && !m1message1Printed && !m1message2Printed)
                    m1C2fromB1Lights.Value = true;
                else
                    m1C2fromB1Lights.Value = false;

                if (sistemaDeManufaturaSupervisor.IsInActiveEventsLights("M1_c3") && !m1message1Printed && !m1message2Printed)
                    m1C3fromB3Lights.Value = true;
                else
                    m1C3fromB3Lights.Value = false;

                if (sistemaDeManufaturaSupervisor.IsInActiveEventsLights("M1_c1") && !m1message1Printed && !m1message2Printed)
                    m1C1fromB1Lights.Value = true;
                else
                    m1C1fromB1Lights.Value = false;
            }
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BUTTON LIGHTS END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% KEYBOARD INPUT START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            {
                if (manual.Value)
                {
                    try
                    {
                        if (!changeStateMessagePrinted)
                        {
                            changeStateMessagePrinted = true;
                        }
                        newStateName = "";
                        newState = Reader.ReadLine(5);
                        try
                        {
                            newStateName = sistemaDeManufaturaSupervisor.StateName(int.Parse(newState));
                            if (newStateName != "Event number pressed does not exist")
                            {
                                if (!sistemaDeManufaturaSupervisor.IsInActiveEvents(int.Parse(newState)))
                                {
                                    Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                                    Console.WriteLine("\nEvent " + newState + " is not in active events. Try again.\n");
                                    Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                                    sistemaDeManufaturaSupervisor.ListOfActiveEvents();
                                }
                            }
                        }
                        catch
                        {
                            Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                            Console.WriteLine("\nSorry, please insert a number.\n");
                            Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                            sistemaDeManufaturaSupervisor.ListOfActiveEvents();
                        }
                        changeStateMessagePrinted = false;
                    }
                    catch (TimeoutException)
                    {
                    }
                }
            }
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% KEYBOARD INPUT END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%% ESTEIRA START %%%%%%%%%%%%%%%%%%%%
            {
                rtSensorStartE2.CLK(sensorStartE2.Value);
                rtSensorPreEndE2.CLK(sensorPreEndE2.Value);
                ftSensorPreEndE2.CLK(sensorPreEndE2.Value);
                ftAfterBlade.CLK(sensorAfterBlade.Value);
                rtAfterBlade.CLK(sensorAfterBlade.Value);
                ftAtRobotArm.CLK(sensorAtRobotArm.Value);
                rtBeforeBuffer.CLK(sensorBeforeBuffer.Value);
                ftBeforeBuffer.CLK(sensorBeforeBuffer.Value);

                if (manual.Value)
                {
                    //E1
                    if (conveyorE1Start.Value || (newStateName == "E1_on" && sistemaDeManufaturaSupervisor.IsInActiveEvents(int.Parse(newState))))
                    {
                        if (e1Counter == 0)
                        {
                            supervisoryApproval = sistemaDeManufaturaSupervisor.On("E1_on");
                            if (supervisoryApproval)
                            {
                                conveyorE1.Value = true;
                                e1Counter++;
                            }
                        }
                    }
                    else if (!conveyorE1Stop.Value || (newStateName == "E1_off" && sistemaDeManufaturaSupervisor.IsInActiveEvents(int.Parse(newState))))
                    {
                        if (e1Counter == 0)
                        {
                            supervisoryApproval = sistemaDeManufaturaSupervisor.On("E1_off");
                            if (supervisoryApproval)
                            {
                                conveyorE1.Value = false;
                                e1Counter++;
                            }
                        }
                    }
                    else
                    {
                        e1Counter = 0;
                    }
                }
                else if (automatic.Value)
                {
                    //E1 auto
                    if (sistemaDeManufaturaSupervisor.IsInActiveEventsString("E1_on"))
                    {
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("E1_on");
                        if (supervisoryApproval)
                        {
                            conveyorE1.Value = true;
                        }
                    }
                    else if (sistemaDeManufaturaSupervisor.IsInActiveEventsString("E1_off"))
                    {
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("E1_off");
                        if (supervisoryApproval)
                        {
                            conveyorE1.Value = false;
                            e1Counter++;
                        }
                    }
                }

                //E1

                //emitterPart
                
                if (e1ConveyorState == E1ConveyorState.EMITTING)
                {
                    randomNumber = random.Next(1, 17);
                    if (randomNumber <= 4)
                        emitterPart.Value = 2048;
                    else if (randomNumber > 4 && randomNumber <= 8)
                        emitterPart.Value = 1024;
                    else if (randomNumber > 8)
                        emitterPart.Value = 512;
                    emitter.Value = true;
                    if (sensorEmitter.Value)
                    {
                        e1ConveyorState = E1ConveyorState.EMITTED;
                    }
                }
                else if (e1ConveyorState == E1ConveyorState.EMITTED)
                {
                    emitter.Value = false;
                    if (sensorEmitter.Value == false)
                    {
                        e1ConveyorState = E1ConveyorState.GOING_TO_ROBO0;
                    }
                }
                else if (e1ConveyorState == E1ConveyorState.GOING_TO_ROBO0)
                {
                    if (sensorEndE1.Value)
                    {
                        e1ConveyorState = E1ConveyorState.REACHED_ROBO0;
                    }
                }
                else if (e1ConveyorState == E1ConveyorState.REACHED_ROBO0)
                {
                    if (!sensorEndE1.Value && !sensorAtRobotArm.Value)
                    {
                        e1ConveyorState = E1ConveyorState.EMITTING;
                    }
                    else if (!sensorEndE1.Value && sensorAtRobotArm.Value)
                    {
                        e1ConveyorState = E1ConveyorState.E2_TO_E1;
                        e2toE1Steps = E2toE1Steps.GOING_TO_E2;
                    }
                }
                else if (e1ConveyorState == E1ConveyorState.E2_TO_E1) //%%%%%%%%%%%% ARM
                {
                    if (e2toE1Steps == E2toE1Steps.GOING_TO_E2)
                    {
                        armX.Value = 2.3f;
                        if (armXpos.Value > 2.2f && armXpos.Value < 2.4f)
                        {
                            e2toE1Steps = E2toE1Steps.DOWN_LOOKING_FOR_PIECE;
                        }
                    }
                    else if (e2toE1Steps == E2toE1Steps.DOWN_LOOKING_FOR_PIECE)
                    {
                        armZ.Value = 8.7f;
                        if (armZpos.Value > 8.6f)
                        {
                            e2toE1Steps = E2toE1Steps.GRABBING_PIECE;
                        }
                    }
                    else if (e2toE1Steps == E2toE1Steps.GRABBING_PIECE)
                    {
                        armGrab.Value = true;
                        armZ.Value = 8.9f;
                        if (armPieceDetected.Value && armZpos.Value > 8.7f)
                        {
                            e2toE1Steps = E2toE1Steps.UP_WITH_PIECE;
                        }
                    }
                    else if (e2toE1Steps == E2toE1Steps.UP_WITH_PIECE)
                    {
                        armZ.Value = 4.0f;
                        if (armZpos.Value < 4.4f)
                        {
                            conveyorEndE2.Value = 1;
                            e2toE1Steps = E2toE1Steps.GOING_TO_E1_FIRST_HALF;
                            rotationBool = false;
                        }
                    }
                    else if (e2toE1Steps == E2toE1Steps.GOING_TO_E1_FIRST_HALF)
                    {
                        armRotate.Value = true;
                        armPieceRotate.Value = true;
                        armX.Value = 4.0f;
                        if (rotationBool)
                        {
                            Thread.Sleep(200);
                            armRotate.Value = false;
                            armPieceRotate.Value = false;
                        }
                        else
                        {
                            rotationBool = true;
                        }
                        if (!armRotating.Value && armXpos.Value > 3.9f && rotationBool)
                        {
                            e2toE1Steps = E2toE1Steps.GOING_TO_E1_SECOND_HALF;
                            rotationBool = false;
                        }
                    }
                    else if (e2toE1Steps == E2toE1Steps.GOING_TO_E1_SECOND_HALF)
                    {
                        armRotate.Value = true;
                        armX.Value = 5.0f;
                        if (!armRotating.Value && armXpos.Value > 4.9f && rotationBool)
                        {
                            Thread.Sleep(200);
                            armRotate.Value = false;
                            e2toE1Steps = E2toE1Steps.DOWN_WITH_PIECE;
                        }
                        else
                        {
                            rotationBool = true;
                        }
                    }
                    else if (e2toE1Steps == E2toE1Steps.DOWN_WITH_PIECE)
                    {
                        armZ.Value = 9.0f;
                        if (armZpos.Value > 8.9f)
                        {
                            e2toE1Steps = E2toE1Steps.UNGRABBING_PIECE;
                        }
                    }
                    else if (e2toE1Steps == E2toE1Steps.UNGRABBING_PIECE)
                    {
                        armGrab.Value = false;
                        e2toE1Steps = E2toE1Steps.UP_WITHOUT_PIECE;
                    }
                    else if (e2toE1Steps == E2toE1Steps.UP_WITHOUT_PIECE)
                    {
                        armZ.Value = 5.5f;
                        if (armZpos.Value < 5.6f)
                        {
                            e2toE1Steps = E2toE1Steps.UNROTATE_FIRST_HALF;
                            rotationBool = false;
                        }
                    }
                    else if (e2toE1Steps == E2toE1Steps.UNROTATE_FIRST_HALF)
                    {
                        armUnrotate.Value = true;
                        armZ.Value = 4.5f;
                        if (!armRotating.Value && armZpos.Value < 4.6f && armZpos.Value > 4.4f && rotationBool)
                        {
                            armUnrotate.Value = false;
                            e2toE1Steps = E2toE1Steps.UNROTATE_SECOND_HALF;
                            rotationBool = false;
                        }
                        else
                        {
                            Thread.Sleep(200);
                            rotationBool = true;
                        }
                    }
                    else if (e2toE1Steps == E2toE1Steps.UNROTATE_SECOND_HALF)
                    {
                        armUnrotate.Value = true;
                        armZ.Value = 3.5f;
                        if (!armRotating.Value && armZpos.Value < 3.6f && armZpos.Value > 3.4f && rotationBool)
                        {
                            armUnrotate.Value = false;
                            e1ConveyorState = E1ConveyorState.EMITTED;
                            armX.Value = 2.3f;
                            armZ.Value = 6.0f;
                        }
                        else
                        {
                            Thread.Sleep(200);
                            rotationBool = true;
                        }
                    }
                }

                //E2

                E2Loader();
                E2conveyors();

                if (bufferE2 == BufferE2.ZERO)
                {
                    if (rtSensorStartE2.Q)//Piece arrives to E2
                    {
                        bufferE2 = BufferE2.ONE;
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_ON;
                    }
                }
                else if (bufferE2 == BufferE2.ONE)
                {
                    if (ftAtRobotArm.Q)//Robot arm takes piece from E2 to E1
                    {
                        bufferE2 = BufferE2.ZERO;
                    }
                    else if (rtSensorStartE2.Q)//Piece arrives to E2
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_ON;
                        bufferE2 = BufferE2.TWO;
                    }

                    if (ftBeforeBuffer.Q)//Piece arrives to before buffer sensor
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_OFF;
                        e2BeforeBuffer = E2BeforeBuffer.TAKING_PIECES_TO_BUFFER;
                        e2PieceCounter++;
                    }

                    if (rtSensorPreEndE2.Q)//Piece arrives to stopblade
                    {
                        e2BeforeBuffer = E2BeforeBuffer.TURN_OFF_CONVEYORS;
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                    }

                    if (ftAfterBlade.Q)
                        e2PieceCounter--;
                }
                else if (bufferE2 == BufferE2.TWO)
                {
                    if (rtSensorStartE2.Q)//Piece arrives to E2
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_ON;
                        bufferE2 = BufferE2.THREE;
                    }
                    else if (ftAtRobotArm.Q)//Robot arm takes piece from E2 to E1
                    {
                        bufferE2 = BufferE2.ONE;
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                    }
                    else if (rtBeforeBuffer.Q)//Piece arrives to before buffer sensor
                    {
                        if (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM)
                            e2BeforeBuffer = E2BeforeBuffer.TAKING_PIECES_TO_BUFFER;
                        else
                            e2BeforeBuffer = E2BeforeBuffer.WAITING;
                        e2PieceCounter++;
                    }
                    else if (sensorPreEndE2.Value && !sensorAtRobotArm.Value && bufferE2LoadingStage == BufferE2LoadingStage.IDLE)//First piece incoming
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                    }
                    else if (sensorPreEndE2.Value && !sensorSecondSpotE2.Value && (sensorAtRobotArm.Value || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_OFF;
                        e2BeforeBuffer = E2BeforeBuffer.TURN_OFF_CONVEYORS;
                    }
                    
                    if (ftAfterBlade.Q)
                        e2PieceCounter--;
                }
                else if (bufferE2 == BufferE2.THREE)
                {
                    if (rtSensorStartE2.Q)//Piece arrives to E2
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_ON;
                        bufferE2 = BufferE2.FOUR;
                    }
                    else if (ftAtRobotArm.Q)//Robot arm takes piece from E2 to E1
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                        bufferE2 = BufferE2.TWO;
                    }
                    else if (rtBeforeBuffer.Q)//Piece arrives to before buffer sensor
                    {
                        if (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM)
                            e2BeforeBuffer = E2BeforeBuffer.TAKING_PIECES_TO_BUFFER;
                        else
                            e2BeforeBuffer = E2BeforeBuffer.WAITING;
                        e2PieceCounter++;
                    }
                    else if (sensorPreEndE2.Value && !sensorAtRobotArm.Value && bufferE2LoadingStage == BufferE2LoadingStage.IDLE)//First piece incoming
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                    }
                    else if (sensorPreEndE2.Value && sensorSecondSpotE2.Value && !sensorThirdSpotE2.Value && (sensorAtRobotArm.Value || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_OFF;
                        e2BeforeBuffer = E2BeforeBuffer.TURN_OFF_CONVEYORS;
                    }

                    if (ftAfterBlade.Q)
                        e2PieceCounter--;
                }
                else if (bufferE2 == BufferE2.FOUR)
                {
                    if (rtSensorStartE2.Q)//Piece arrives to E2
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_ON;
                        bufferE2 = BufferE2.FIVE;
                    }
                    else if (ftAtRobotArm.Q)//Robot arm takes piece from E2 to E1
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                        bufferE2 = BufferE2.THREE;
                    }
                    else if (rtBeforeBuffer.Q)//Piece arrives to before buffer sensor
                    {
                        if (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM)
                            e2BeforeBuffer = E2BeforeBuffer.TAKING_PIECES_TO_BUFFER;
                        else
                            e2BeforeBuffer = E2BeforeBuffer.WAITING;
                        e2PieceCounter++;
                    }
                    else if (sensorPreEndE2.Value && !sensorAtRobotArm.Value && bufferE2LoadingStage == BufferE2LoadingStage.IDLE)//First piece incoming
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                    }
                    else if (sensorPreEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && !sensorFourthSpotE2.Value && (sensorAtRobotArm.Value || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_OFF;
                        e2BeforeBuffer = E2BeforeBuffer.TURN_OFF_CONVEYORS;
                    }

                    if (ftAfterBlade.Q)
                        e2PieceCounter--;
                }
                else if (bufferE2 == BufferE2.FIVE)
                {
                    if (rtSensorStartE2.Q)//Piece arrives to E2
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_ON;
                        bufferE2 = BufferE2.SIX;
                    }
                    else if (ftAtRobotArm.Q)//Robot arm takes piece from E2 to E1
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                        bufferE2 = BufferE2.FOUR;
                    }
                    else if (rtBeforeBuffer.Q)//Piece arrives to before buffer sensor
                    {
                        if (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM)
                            e2BeforeBuffer = E2BeforeBuffer.TAKING_PIECES_TO_BUFFER;
                        else
                            e2BeforeBuffer = E2BeforeBuffer.WAITING;
                        e2PieceCounter++;
                    }
                    else if (sensorPreEndE2.Value && !sensorAtRobotArm.Value && bufferE2LoadingStage == BufferE2LoadingStage.IDLE)//First piece incoming
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                    }
                    else if (sensorPreEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && !sensorFifthSpotE2.Value && (sensorAtRobotArm.Value || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_OFF;
                        e2BeforeBuffer = E2BeforeBuffer.TURN_OFF_CONVEYORS;
                    }

                    if (ftAfterBlade.Q)
                        e2PieceCounter--;
                }
                else if (bufferE2 == BufferE2.SIX)
                {
                    if (rtSensorStartE2.Q)//Piece arrives to E2
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_ON;
                        bufferE2 = BufferE2.SEVEN;
                    }
                    else if (ftAtRobotArm.Q)//Robot arm takes piece from E2 to E1
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                        bufferE2 = BufferE2.FIVE;
                    }
                    else if (rtBeforeBuffer.Q)//Piece arrives to before buffer sensor
                    {
                        if (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM)
                            e2BeforeBuffer = E2BeforeBuffer.TAKING_PIECES_TO_BUFFER;
                        else
                            e2BeforeBuffer = E2BeforeBuffer.WAITING;
                        e2PieceCounter++;
                    }
                    else if (sensorPreEndE2.Value && !sensorAtRobotArm.Value && bufferE2LoadingStage == BufferE2LoadingStage.IDLE)//First piece incoming
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                    }
                    else if (sensorPreEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && sensorFifthSpotE2.Value && !sensorSixthSpotE2.Value && (sensorAtRobotArm.Value || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_OFF;
                        e2BeforeBuffer = E2BeforeBuffer.TURN_OFF_CONVEYORS;
                    }

                    if (ftAfterBlade.Q)
                        e2PieceCounter--;
                }
                else if (bufferE2 == BufferE2.SEVEN)
                {
                    if (rtSensorStartE2.Q)//Piece arrives to E2
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_ON;
                        bufferE2 = BufferE2.EIGHT;
                    }
                    else if (ftAtRobotArm.Q)//Robot arm takes piece from E2 to E1
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                        bufferE2 = BufferE2.SIX;
                    }
                    else if (rtBeforeBuffer.Q)//Piece arrives to before buffer sensor
                    {
                        if (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM)
                            e2BeforeBuffer = E2BeforeBuffer.TAKING_PIECES_TO_BUFFER;
                        else
                            e2BeforeBuffer = E2BeforeBuffer.WAITING;
                        e2PieceCounter++;
                    }
                    else if (sensorPreEndE2.Value && !sensorAtRobotArm.Value && bufferE2LoadingStage == BufferE2LoadingStage.IDLE)//First piece incoming
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                    }
                    else if (sensorPreEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && sensorFifthSpotE2.Value && sensorSixthSpotE2.Value && !sensorSeventhSpotE2.Value && (sensorAtRobotArm.Value || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_OFF;
                        e2BeforeBuffer = E2BeforeBuffer.TURN_OFF_CONVEYORS;
                    }

                    if (ftAfterBlade.Q)
                        e2PieceCounter--;
                }
                else if (bufferE2 == BufferE2.EIGHT)
                {
                    if (rtSensorStartE2.Q)//Piece arrives to E2
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_ON;
                        bufferE2 = BufferE2.NINE;
                    }
                    else if (ftAtRobotArm.Q)//Robot arm takes piece from E2 to E1
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                        bufferE2 = BufferE2.SEVEN;
                    }
                    else if (rtBeforeBuffer.Q)//Piece arrives to before buffer sensor
                    {
                        if (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM)
                            e2BeforeBuffer = E2BeforeBuffer.TAKING_PIECES_TO_BUFFER;
                        else
                            e2BeforeBuffer = E2BeforeBuffer.WAITING;
                        e2PieceCounter++;
                    }
                    else if (sensorPreEndE2.Value && !sensorAtRobotArm.Value && bufferE2LoadingStage == BufferE2LoadingStage.IDLE)//First piece incoming
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                    }
                    else if (sensorPreEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value &&
                        sensorFifthSpotE2.Value && sensorSixthSpotE2.Value && sensorSeventhSpotE2.Value && !sensorEighthSpotE2.Value &&
                        (sensorAtRobotArm.Value || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_OFF;
                        e2BeforeBuffer = E2BeforeBuffer.TURN_OFF_CONVEYORS;
                    }

                    if (ftAfterBlade.Q)
                        e2PieceCounter--;
                }
                else if (bufferE2 == BufferE2.NINE)
                {
                    if (rtSensorStartE2.Q)//Piece arrives to E2
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_ON;
                        bufferE2 = BufferE2.TEN;
                    }
                    else if (ftAtRobotArm.Q)//Robot arm takes piece from E2 to E1
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                        bufferE2 = BufferE2.EIGHT;
                    }
                    else if (rtBeforeBuffer.Q)//Piece arrives to before buffer sensor
                    {
                        if (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM)
                            e2BeforeBuffer = E2BeforeBuffer.TAKING_PIECES_TO_BUFFER;
                        else
                            e2BeforeBuffer = E2BeforeBuffer.WAITING;
                        e2PieceCounter++;
                    }
                    else if (sensorPreEndE2.Value && !sensorAtRobotArm.Value && bufferE2LoadingStage == BufferE2LoadingStage.IDLE)//First piece incoming
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                    }
                    else if (sensorPreEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value &&
                        sensorFifthSpotE2.Value && sensorSixthSpotE2.Value && sensorSeventhSpotE2.Value && sensorEighthSpotE2.Value &&
                        !sensorNinthSpotE2.Value && (sensorAtRobotArm.Value || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_OFF;
                        e2BeforeBuffer = E2BeforeBuffer.TURN_OFF_CONVEYORS;
                    }

                    if (ftAfterBlade.Q)
                        e2PieceCounter--;
                }
                else if (bufferE2 == BufferE2.TEN)
                {
                    if (rtSensorStartE2.Q)//Piece arrives to E2
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_ON;
                        bufferE2 = BufferE2.ELEVEN;
                    }
                    else if (ftAtRobotArm.Q)//Robot arm takes piece from E2 to E1
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                        bufferE2 = BufferE2.NINE;
                    }
                    else if (rtBeforeBuffer.Q)//Piece arrives to before buffer sensor
                    {
                        if (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM)
                            e2BeforeBuffer = E2BeforeBuffer.TAKING_PIECES_TO_BUFFER;
                        else
                            e2BeforeBuffer = E2BeforeBuffer.WAITING;
                        e2PieceCounter++;
                    }
                    else if (sensorPreEndE2.Value && !sensorAtRobotArm.Value && bufferE2LoadingStage == BufferE2LoadingStage.IDLE)//First piece incoming
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                    }
                    else if (sensorPreEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value &&
                        sensorFifthSpotE2.Value && sensorSixthSpotE2.Value && sensorSeventhSpotE2.Value && sensorEighthSpotE2.Value &&
                        sensorNinthSpotE2.Value && !sensorTenthSpotE2.Value &&
                        (sensorAtRobotArm.Value || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_OFF;
                        e2BeforeBuffer = E2BeforeBuffer.TURN_OFF_CONVEYORS;
                    }

                    if (ftAfterBlade.Q)
                        e2PieceCounter--;
                }
                else if (bufferE2 == BufferE2.ELEVEN)
                {
                    if (ftAtRobotArm.Q)//Robot arm takes piece from E2 to E1
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                        bufferE2 = BufferE2.TEN;
                    }
                    else if (rtBeforeBuffer.Q)//Piece arrives to before buffer sensor
                    {
                        if (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM)
                            e2BeforeBuffer = E2BeforeBuffer.TAKING_PIECES_TO_BUFFER;
                        else
                            e2BeforeBuffer = E2BeforeBuffer.WAITING;
                        e2PieceCounter++;
                    }
                    else if (sensorPreEndE2.Value && !sensorAtRobotArm.Value && bufferE2LoadingStage == BufferE2LoadingStage.IDLE)//First piece incoming
                    {
                        bufferE2LoadingStage = BufferE2LoadingStage.START_LOADING;
                    }
                    else if (sensorPreEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value &&
                        sensorFifthSpotE2.Value && sensorSixthSpotE2.Value && sensorSeventhSpotE2.Value && sensorEighthSpotE2.Value &&
                        sensorNinthSpotE2.Value && sensorTenthSpotE2.Value &&
                        (sensorAtRobotArm.Value || bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        e2ConveyorsOnOff = E2ConveyorsOnOff.CONVEYORS_OFF;
                        e2BeforeBuffer = E2BeforeBuffer.TURN_OFF_CONVEYORS;
                    }

                    if (ftAfterBlade.Q)
                        e2PieceCounter--;
                }

                if (e2BeforeBuffer == E2BeforeBuffer.TAKING_PIECES_TO_BUFFER)
                {
                    conveyorPrePreEndE2.Value = 1.0f;
                    conveyorPreEndE2.Value = 1.0f;
                    e2BeforeBuffer = E2BeforeBuffer.IDLE;
                }
                else if (e2BeforeBuffer == E2BeforeBuffer.WAITING)
                {
                    if (bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM || bufferE2LoadingStage == BufferE2LoadingStage.IDLE)
                        e2BeforeBuffer = E2BeforeBuffer.TAKING_PIECES_TO_BUFFER;
                }
                else if (e2BeforeBuffer == E2BeforeBuffer.TURN_OFF_CONVEYORS)
                {
                    conveyorPrePreEndE2.Value = 0;
                    conveyorPreEndE2.Value = 0;
                    e2BeforeBuffer = E2BeforeBuffer.IDLE;
                }
                else if (e2BeforeBuffer == E2BeforeBuffer.IDLE)
                {
                }
                if (bufferE2 != BufferE2.ONE)
                {
                    if (e2PieceCounter == 2 && (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage != BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        if (sensorEndE1.Value && sensorSecondSpotE2.Value)
                        {
                            conveyorPreEndE2.Value = 0;
                        }
                    }
                    else if (e2PieceCounter == 3 && (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage != BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        if (sensorEndE1.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value)
                        {
                            conveyorPreEndE2.Value = 0;
                        }
                    }
                    else if (e2PieceCounter == 4 && (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage != BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        if (sensorEndE1.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value)
                        {
                            conveyorPreEndE2.Value = 0;
                        }
                    }
                    else if (e2PieceCounter == 5 && (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage != BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        if (sensorEndE1.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && sensorFifthSpotE2.Value)
                        {
                            conveyorPreEndE2.Value = 0;
                        }

                    }
                    else if (e2PieceCounter == 6 && (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage != BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        if (sensorEndE1.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && sensorFifthSpotE2.Value &&
                            sensorSixthSpotE2.Value)
                        {
                            conveyorPreEndE2.Value = 0;
                        }
                    }
                    else if (e2PieceCounter == 7 && (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage != BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        if (sensorEndE1.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && sensorFifthSpotE2.Value && 
                            sensorSixthSpotE2.Value && sensorSeventhSpotE2.Value)
                        {
                            conveyorPreEndE2.Value = 0;
                        }
                    }
                    else if (e2PieceCounter == 8 && (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage != BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        if (sensorEndE1.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && sensorFifthSpotE2.Value &&
                            sensorSixthSpotE2.Value && sensorSeventhSpotE2.Value && sensorEighthSpotE2.Value)
                        {
                            conveyorPreEndE2.Value = 0;
                        }
                            
                    }
                    else if (e2PieceCounter == 9 && (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage != BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        if (sensorEndE1.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && sensorFifthSpotE2.Value &&
                            sensorSixthSpotE2.Value && sensorSeventhSpotE2.Value && sensorEighthSpotE2.Value && sensorNinthSpotE2.Value)
                            conveyorPreEndE2.Value = 0;
                    }
                    else if (e2PieceCounter == 10 && (bufferE2LoadingStage == BufferE2LoadingStage.IDLE || bufferE2LoadingStage != BufferE2LoadingStage.REACHING_ROBOT_ARM))
                    {
                        if (sensorEndE1.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && sensorFifthSpotE2.Value &&
                            sensorSixthSpotE2.Value && sensorSeventhSpotE2.Value && sensorEighthSpotE2.Value && sensorNinthSpotE2.Value)
                            conveyorPreEndE2.Value = 0;
                    }
                }
                

            }
            //%%%%%%%%%%%%%%%%%%%% ESTEIRA ENDS %%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%% ROBÔ STARTS %%%%%%%%%%%%%%%%%%%%
            {
                if (manual.Value)
                {
                    if (startC1toE2.Value || (newStateName == "R_c1_e2" && sistemaDeManufaturaSupervisor.IsInActiveEvents(int.Parse(newState))))
                    {
                        if (roboCounter == 0)
                        {
                            supervisoryApproval = sistemaDeManufaturaSupervisor.On("R_c1_e2");
                            if (supervisoryApproval)
                            {
                                robo0State = Robo0State.E1toE2;
                                roboCounter++;
                            }
                        }
                    }
                    else if (startC2toE2.Value || (newStateName == "R_c2_e2" && sistemaDeManufaturaSupervisor.IsInActiveEvents(int.Parse(newState))))
                    {
                        if (roboCounter == 0)
                        {
                            supervisoryApproval = sistemaDeManufaturaSupervisor.On("R_c2_e2");
                            if (supervisoryApproval)
                            {
                                robo0State = Robo0State.E1toE2;
                                roboCounter++;
                            }
                        }
                    }
                    else if (startC3toE2.Value || (newStateName == "R_c3_e2" && sistemaDeManufaturaSupervisor.IsInActiveEvents(int.Parse(newState))))
                    {
                        if (roboCounter == 0)
                        {
                            supervisoryApproval = sistemaDeManufaturaSupervisor.On("R_c3_e2");
                            if (supervisoryApproval)
                            {
                                robo0State = Robo0State.E1toE2;
                                roboCounter++;
                            }
                        }
                    }
                    else if (startC1toB1.Value || (newStateName == "R_c1_b1" && sistemaDeManufaturaSupervisor.IsInActiveEvents(int.Parse(newState))))
                    {
                        if (roboCounter == 0)
                        {
                            supervisoryApproval = sistemaDeManufaturaSupervisor.On("R_c1_b1");
                            if (supervisoryApproval)
                            {
                                robo0State = Robo0State.E1toB1;
                                roboCounter++;
                                color = "c1";
                            }
                        }
                    }
                    else if (startC2toB1.Value || (newStateName == "R_c2_b1" && sistemaDeManufaturaSupervisor.IsInActiveEvents(int.Parse(newState))))
                    {
                        if (roboCounter == 0)
                        {
                            supervisoryApproval = sistemaDeManufaturaSupervisor.On("R_c2_b1");
                            if (supervisoryApproval)
                            {
                                robo0State = Robo0State.E1toB1;
                                roboCounter++;
                                color = "c2";
                            }
                        }
                    }
                    else if (startC2toB2.Value || (newStateName == "R_c2_b2" && sistemaDeManufaturaSupervisor.IsInActiveEvents(int.Parse(newState))))
                    {
                        if (roboCounter == 0)
                        {
                            supervisoryApproval = sistemaDeManufaturaSupervisor.On("R_c2_b2");
                            if (supervisoryApproval)
                            {
                                robo0State = Robo0State.E1toB2;
                                roboCounter++;
                            }
                        }
                    }
                    else if (startC3toB3.Value || (newStateName == "R_c3_b3" && sistemaDeManufaturaSupervisor.IsInActiveEvents(int.Parse(newState))))
                    {
                        if (roboCounter == 0)
                        {
                            supervisoryApproval = sistemaDeManufaturaSupervisor.On("R_c3_b3");
                            if (supervisoryApproval)
                            {
                                robo0State = Robo0State.E1toB3;
                                roboCounter++;
                            }
                        }
                    }
                    else
                    {
                        roboCounter = 0;
                    }
                }
                else if (automatic.Value)//Automatic
                {
                    if (sistemaDeManufaturaSupervisor.IsInActiveEventsString("R_c1_e2"))
                    {
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("R_c1_e2");
                        if (supervisoryApproval)
                        {
                            robo0State = Robo0State.E1toE2;
                        }
                    }
                    else if (sistemaDeManufaturaSupervisor.IsInActiveEventsString("R_c2_e2"))
                    {
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("R_c2_e2");
                        if (supervisoryApproval)
                        {
                            robo0State = Robo0State.E1toE2;
                        }
                    }
                    else if (sistemaDeManufaturaSupervisor.IsInActiveEventsString("R_c3_e2"))
                    {
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("R_c3_e2");
                        if (supervisoryApproval)
                        {
                            robo0State = Robo0State.E1toE2;
                        }
                    }
                    else if (sistemaDeManufaturaSupervisor.IsInActiveEventsString("R_c1_b1"))
                    {
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("R_c1_b1");
                        if (supervisoryApproval)
                        {
                            robo0State = Robo0State.E1toB1;
                            color = "c1";
                        }
                    }
                    else if (sistemaDeManufaturaSupervisor.IsInActiveEventsString("R_c2_b1"))
                    {
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("R_c2_b1");
                        if (supervisoryApproval)
                        {
                            robo0State = Robo0State.E1toB1;
                            color = "c2";
                        }
                    }
                    else if (sistemaDeManufaturaSupervisor.IsInActiveEventsString("R_c2_b2"))
                    {
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("R_c2_b2");
                        if (supervisoryApproval)
                        {
                            robo0State = Robo0State.E1toB2;
                        }
                    }
                    else if (sistemaDeManufaturaSupervisor.IsInActiveEventsString("R_c3_b3"))
                    {
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("R_c3_b3");
                        if (supervisoryApproval)
                        {
                            robo0State = Robo0State.E1toB3;
                        }
                    }
                }

                if (robo0State == Robo0State.E1toE2)
                {
                    roboFinished = robo0.E1toE2();
                    if (roboFinished)
                    {
                        robo0State = Robo0State.IDLE;
                    }
                }
                else if (robo0State == Robo0State.E1toB1)
                {
                    roboFinished = robo0.E1toB1(color);
                    if (roboFinished)
                    {
                        robo0State = Robo0State.IDLE;
                    }
                }
                else if (robo0State == Robo0State.E1toB2)
                {
                    roboFinished = robo0.E1toB2();
                    if (roboFinished)
                    {
                        robo0State = Robo0State.IDLE;
                    }
                }
                else if (robo0State == Robo0State.E1toB3)
                {
                    roboFinished = robo0.E1toB3();
                    if (roboFinished)
                    {
                        robo0State = Robo0State.IDLE;
                    }
                }
                else if (robo0State == Robo0State.IDLE)
                {
                    robo0.Idle();
                }
            }
            //%%%%%%%%%%%%%%%%%%%% ROBÔ ENDS %%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%% COLOR SENSOR STARTS %%%%%%%%%%%%%%%%%%%%
            {
                if (sensorColor.Value == 5)
                {
                    if (colorMessage)
                    {
                        sistemaDeManufaturaSupervisor.On("S_c1");
                        colorMessage = false;
                        colorMessage2 = true;
                    }
                }
                else if (sensorColor.Value == 9)
                {
                    if (colorMessage)
                    {
                        sistemaDeManufaturaSupervisor.On("S_c2");
                        colorMessage = false;
                        colorMessage2 = true;
                    }
                }
                else if (sensorColor.Value == 2)
                {
                    if (colorMessage)
                    {
                        sistemaDeManufaturaSupervisor.On("S_c3");
                        colorMessage = false;
                        colorMessage2 = true;
                    }
                }
                else if (sensorColor.Value == 0)
                {
                    if (colorMessage2)
                    {
                        sistemaDeManufaturaSupervisor.On("S_nada");
                        colorMessage2 = false;
                    }
                    colorMessage = true;
                }
            }
            //%%%%%%%%%%%%%%%%%%%% COLOR SENSOR ENDS %%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%% M1 STARTS %%%%%%%%%%%%%%%%%%%%
            {
                roboM1.Takeaway();
                ftAtM1end.CLK(sensorM1end.Value);

                if (manual.Value)
                {
                    if ((startC1fromB1toM1.Value && m1states == M1states.IDLE) || (newStateName == "M1_c1" && sistemaDeManufaturaSupervisor.IsInActiveEvents(int.Parse(newState)) && m1states == M1states.IDLE))
                    {
                        if (m1Counter == 0)
                        {
                            supervisoryApproval = sistemaDeManufaturaSupervisor.On("M1_c1");
                            if (supervisoryApproval)
                            {
                                m1states = M1states.C1fromB1toM1;
                                m1Counter++;
                            }
                        }
                    }
                    else if ((startC2fromB1toM1.Value && m1states == M1states.IDLE) || (newStateName == "M1_c2_b1" && sistemaDeManufaturaSupervisor.IsInActiveEvents(int.Parse(newState)) && m1states == M1states.IDLE))
                    {
                        if (m1Counter == 0)
                        {
                            supervisoryApproval = sistemaDeManufaturaSupervisor.On("M1_c2_b1");
                            if (supervisoryApproval)
                            {
                                m1states = M1states.C2fromB1toM1;
                                m1Counter++;
                            }
                        }
                    }
                    else if ((startC2fromB2toM1.Value && m1states == M1states.IDLE) || (newStateName == "M1_c2" && sistemaDeManufaturaSupervisor.IsInActiveEvents(int.Parse(newState)) && m1states == M1states.IDLE))
                    {
                        if (m1Counter == 0)
                        {
                            supervisoryApproval = sistemaDeManufaturaSupervisor.On("M1_c2");
                            if (supervisoryApproval)
                            {
                                m1states = M1states.C2fromB2toM1;
                                m1Counter++;
                            }
                        }
                    }
                    else if ((startC3fromB3toM1.Value && m1states == M1states.IDLE) || (newStateName == "M1_c3" && sistemaDeManufaturaSupervisor.IsInActiveEvents(int.Parse(newState)) && m1states == M1states.IDLE))
                    {
                        if (m1Counter == 0)
                        {
                            supervisoryApproval = sistemaDeManufaturaSupervisor.On("M1_c3");
                            if (supervisoryApproval)
                            {
                                m1states = M1states.C3fromB3toM1;
                                m1Counter++;
                            }
                        }
                    }
                    else
                    {
                        m1Counter = 0;
                    }
                }
                else if (automatic.Value)//Auto
                {
                    if (sistemaDeManufaturaSupervisor.IsInActiveEventsString("M1_c1") && m1states == M1states.IDLE)
                    {
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("M1_c1");
                        if (supervisoryApproval)
                        {
                            m1states = M1states.C1fromB1toM1;
                        }
                    }
                    else if (sistemaDeManufaturaSupervisor.IsInActiveEventsString("M1_c2_b1") && m1states == M1states.IDLE)
                    {
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("M1_c2_b1");
                        if (supervisoryApproval)
                        {
                            m1states = M1states.C2fromB1toM1;
                        }
                    }
                    else if (sistemaDeManufaturaSupervisor.IsInActiveEventsString("M1_c2") && m1states == M1states.IDLE)
                    {
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("M1_c2");
                        if (supervisoryApproval)
                        {
                            m1states = M1states.C2fromB2toM1;
                        }
                    }
                    else if (sistemaDeManufaturaSupervisor.IsInActiveEventsString("M1_c3") && m1states == M1states.IDLE)
                    {
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("M1_c3");
                        if (supervisoryApproval)
                        {
                            m1states = M1states.C3fromB3toM1;
                        }
                    }
                }



                if (m1states == M1states.IDLE)
                {
                    roboM1.Idle();
                }
                else if (m1states == M1states.C1fromB1toM1)
                {
                    if ((newStateName == "M1_c2" || startC2fromB2toM1.Value) && !m1message1Printed)
                    {
                        Console.WriteLine("////////////////////////////////////////\n");
                        Console.WriteLine("Please wait...");
                        Console.WriteLine("////////////////////////////////////////");
                        bool_M1_c2 = true;
                        m1message1Printed = true;
                    }
                    roboM1Finished = roboM1.C1fromB1toM1();
                    if (roboM1Finished && bool_M1_c2)
                    {
                        roboM1.Idle();
                        bool_M1_c2 = false;
                        m1message1Printed = false;
                        sistemaDeManufaturaSupervisor.On("M1_fin");
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("M1_c2");
                        if (supervisoryApproval)
                        {
                            m1states = M1states.C2fromB2toM1;
                        }
                        else
                        {
                            newStateName = "";
                            m1states = M1states.IDLE;
                        }
                    }
                    else if (roboM1Finished)
                    {
                        m1states = M1states.IDLE;
                        sistemaDeManufaturaSupervisor.On("M1_fin");
                    }

                }
                else if (m1states == M1states.C2fromB1toM1)
                {
                    roboM1Finished = roboM1.C2fromB1toM1();
                    if ((newStateName == "M1_c3" || startC3fromB3toM1.Value) && !m1message1Printed)
                    {
                        m1C3fromB3Lights.Value = false;
                        Console.WriteLine("////////////////////////////////////////\n");
                        Console.WriteLine("Please wait...");
                        Console.WriteLine("////////////////////////////////////////");
                        bool_M1_c3 = true;
                        m1message1Printed = true;
                    }
                    if (roboM1Finished && bool_M1_c3)
                    {
                        roboM1.Idle();
                        bool_M1_c3 = false;
                        m1message1Printed = false;
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("M1_c3");
                        if (supervisoryApproval)
                        {
                            m1states = M1states.C3fromB3toM1;
                        }
                        else
                        {
                            m1states = M1states.IDLE;
                            newStateName = "";
                        }
                    }
                    else if (roboM1Finished)
                    {
                        m1states = M1states.IDLE;
                    }
                }
                else if (m1states == M1states.C2fromB2toM1)
                {
                    roboM1Finished = roboM1.C2fromB2toM1();
                    if ((newStateName == "M1_c1" || startC1fromB1toM1.Value) && !m1message1Printed && !m1message2Printed)
                    {
                        m1C1fromB1Lights.Value = false;
                        m1C3fromB3Lights.Value = false;
                        bool_M1_c1 = true;
                        Console.WriteLine("////////////////////////////////////////\n");
                        Console.WriteLine("Please wait...");
                        Console.WriteLine("////////////////////////////////////////");
                        m1message1Printed = true;
                    }
                    else if ((newStateName == "M1_c3" || startC3fromB3toM1.Value) && !m1message2Printed && !m1message1Printed)
                    {
                        m1C3fromB3Lights.Value = false;
                        m1C1fromB1Lights.Value = false;
                        bool_M1_c3 = true;
                        Console.WriteLine("////////////////////////////////////////\n");
                        Console.WriteLine("Please wait...");
                        Console.WriteLine("////////////////////////////////////////");
                        m1message2Printed = true;
                    }
                    if (roboM1Finished && bool_M1_c1)
                    {
                        roboM1.Idle();
                        bool_M1_c1 = false;
                        m1message1Printed = false;
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("M1_c1");
                        if (supervisoryApproval)
                        {
                            m1states = M1states.C1fromB1toM1;
                        }
                        else
                        {
                            m1states = M1states.IDLE;
                            newStateName = "";
                        }
                    }
                    else if (roboM1Finished && bool_M1_c3)
                    {
                        roboM1.Idle();
                        bool_M1_c3 = false;
                        m1message2Printed = false;
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("M1_c3");
                        if (supervisoryApproval)
                        {
                            m1states = M1states.C3fromB3toM1;
                        }
                        else
                        {
                            m1states = M1states.IDLE;
                            newStateName = "";
                        }
                    }
                    else if (roboM1Finished)
                    {
                        m1states = M1states.IDLE;
                    }
                }
                else if (m1states == M1states.C3fromB3toM1)
                {
                    roboM1Finished = roboM1.C3fromB3toM1();
                    if ((newStateName == "M1_c2" || startC2fromB2toM1.Value) && !m1message1Printed && !m1message2Printed)
                    {
                        m1C2fromB2Lights.Value = false;
                        Console.WriteLine("////////////////////////////////////////\n");
                        Console.WriteLine("Please wait...");
                        Console.WriteLine("////////////////////////////////////////");
                        bool_M1_c2 = true;
                        m1message1Printed = true;
                    }
                    else if ((newStateName == "M1_c2_b1" || startC2fromB1toM1.Value) && !m1message2Printed && !m1message1Printed)
                    {
                        m1C2fromB1Lights.Value = false;
                        Console.WriteLine("////////////////////////////////////////\n");
                        Console.WriteLine("Please wait...");
                        Console.WriteLine("////////////////////////////////////////");
                        bool_M1_c2_b1 = true;
                        m1message2Printed = true;
                    }
                    if (roboM1Finished && bool_M1_c2)
                    {
                        roboM1.Idle();
                        bool_M1_c2 = false;
                        m1message1Printed = false;
                        Console.WriteLine("RoboM1Finished && newStateName = M1_c2");
                        sistemaDeManufaturaSupervisor.On("M1_fin");
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("M1_c2");
                        if (supervisoryApproval)
                        {
                            m1states = M1states.C2fromB2toM1;
                        }
                        else
                        {
                            m1states = M1states.IDLE;
                            newStateName = "";
                        }
                    }
                    else if (roboM1Finished && bool_M1_c2_b1)
                    {
                        roboM1.Idle();
                        bool_M1_c2_b1 = false;
                        m1message2Printed = false;
                        sistemaDeManufaturaSupervisor.On("M1_fin");
                        supervisoryApproval = sistemaDeManufaturaSupervisor.On("M1_c2_b1");
                        if (supervisoryApproval)
                        {
                            m1states = M1states.C2fromB1toM1;
                        }
                        else
                        {
                            m1states = M1states.IDLE;
                            newStateName = "";
                        }
                    }
                    else if (roboM1Finished)
                    {
                        m1states = M1states.IDLE;
                        sistemaDeManufaturaSupervisor.On("M1_fin");
                    }
                }
            }

            //%%%%%%%%%%%%%%%%%%%% M1 ENDS %%%%%%%%%%%%%%%%%%%%
        }
        //%%%%%%%%%%%%%%%%%%%% CLASSES START %%%%%%%%%%%%%%%%%%%%
        private void E2Loader()
        {
            if (bufferE2LoadingStage == BufferE2LoadingStage.START_LOADING)
            {
                E2Stopblade.Value = false;
                conveyorPreEndE2.Value = 0.7f;
                conveyorEndE2.Value = 1.0f;
                if (rtAfterBlade.Q)
                {
                    bufferE2LoadingStage = BufferE2LoadingStage.SEPARATE_PIECES;
                }
            }
            else if (bufferE2LoadingStage == BufferE2LoadingStage.SEPARATE_PIECES)
            {
                conveyorPreEndE2.Value = 0;
                if (ftAfterBlade.Q)
                {
                    bufferE2LoadingStage = BufferE2LoadingStage.RESTORING_BUFFER_ORDER;
                }
            }
            else if (bufferE2LoadingStage == BufferE2LoadingStage.RESTORING_BUFFER_ORDER)
            {
                E2Stopblade.Value = true;
                conveyorPreEndE2.Value = 1.0f;
                bufferE2LoadingStage = BufferE2LoadingStage.REACHING_ROBOT_ARM;
            }
            else if (bufferE2LoadingStage == BufferE2LoadingStage.REACHING_ROBOT_ARM)
            {
                if (sensorAtRobotArm.Value)
                {
                    bufferE2LoadingStage = BufferE2LoadingStage.IDLE;
                    conveyorEndE2.Value = 0;
                }
            }
            else if (bufferE2LoadingStage == BufferE2LoadingStage.IDLE)
            {
            }
        }

        private void E2conveyors()
        {
            if (e2ConveyorsOnOff == E2ConveyorsOnOff.CONVEYORS_ON)
            {
                conveyorStartE2.Value = 1.0f;
                conveyorPostStartE2.Value = 1.0f;
                conveyorFirstCornerE2.Value = -2.7f;
                conveyorMiddleE2.Value = 1.0f;
                conveyorSecondCornerE2.Value = -2.8f;
                conveyorPrePreEndE2.Value = 1.0f;
            }
            else if (e2ConveyorsOnOff == E2ConveyorsOnOff.CONVEYORS_OFF)
            {
                conveyorStartE2.Value = 0;
                conveyorPostStartE2.Value = 0;
                conveyorFirstCornerE2.Value = 0;
                conveyorMiddleE2.Value = 0;
                conveyorSecondCornerE2.Value = 0;
                conveyorPrePreEndE2.Value = 0;
            }
        }
        //%%%%%%%%%%%%%%%%%%%% CLASSES END %%%%%%%%%%%%%%%%%%%%
    }
}
